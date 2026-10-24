using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;
using System.Transactions;

namespace Investry.Application.Features.Investments.Commands.CreateInvestment
{
    public class CreateInvestmentHandler : IRequestHandler<CreateInvestmentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public CreateInvestmentHandler(IUnitOfWork unitOfWork, IIdentityService identityService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result<Guid>> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdAsync(request.UserId);
            if (user is null)
                return Result<Guid>.Failure(new List<Error> { new Error("User.NotFound", "User not found.", ErrorType.NotFound) });

            if (request.Amount <= 0)
                return Result<Guid>.Failure(new List<Error> { new Error("Investment.InvalidAmount", "Investment amount must be positive.", ErrorType.Validation) });

            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled
            );

            var project = await _unitOfWork.ProjectRepository.GetProjectWithInvestmentDataAsync(request.ProjectId);
            if (project is null)
                return Result<Guid>.Failure(new List<Error> { new Error("Project_Not_Found", "Project not found", ErrorType.NotFound) });

            var now = DateTime.UtcNow;
            if (project.ProjectStatus != ProjectStatus.Published || project.StartDate > now || project.EndDate <= now)
                return Result<Guid>.Failure(new List<Error> { new Error("Project.NotOpen", "This project is not open for investment.", ErrorType.Validation) });

            var investor = await _unitOfWork.InvestorRepository.GetByUserIdAsync(request.UserId);
            if (investor == null)
            {
                investor = new Investor { UserId = request.UserId };
                await _unitOfWork.InvestorRepository.AddAsync(investor);
            }

            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(request.UserId);
            if (wallet is null)
                return Result<Guid>.Failure(new List<Error> { new Error("Wallet_Not_Found", "Wallet not found", ErrorType.NotFound) });

            var escrowWallet = await _unitOfWork.SystemWalletRepository.GetByTypeAsync(SystemWalletType.Escrow);
            if (escrowWallet is null)
                return Result<Guid>.Failure(new List<Error> { new Error("Escrow.NotFound", "Escrow wallet not found.", ErrorType.NotFound) });

            var platformWallet = await _unitOfWork.SystemWalletRepository.GetByTypeAsync(SystemWalletType.Platform);
            if (platformWallet is null)
                return Result<Guid>.Failure(new List<Error> { new Error("Platform.NotFound", "Platform wallet not found.", ErrorType.NotFound) });

            var contributionError = await ValidateContributionAsync(request, project, investor);
            if (contributionError is not null)
                return Result<Guid>.Failure(new List<Error> { contributionError });

            var investmentResult = await CreateInvestmentForFundingModelAsync(request, project, investor, wallet);
            if (investmentResult.IsFailure)
                return Result<Guid>.Failure(investmentResult.Errors);

            var investment = investmentResult.Value;
            var amountToDeduct = investment.Amount;
            await FundAndSaveInvestmentAsync(investment, wallet, escrowWallet);
            var email = InvestmentConfirmationEmail.Create(user, project, amountToDeduct);
            await _emailService.SendAsync(email);

            scope.Complete();
            return Result<Guid>.Success(investment.Id);
        }

        private async Task<Error?> ValidateContributionAsync(CreateInvestmentCommand request, Project project, Investor investor)
        {
            var alreadyInvested = await _unitOfWork.InvestmentRepository.ExistsAsync(investor.Id, project.Id);
            if (alreadyInvested)
                return new Error("Duplicate_Investment", "You already invested in this project.", ErrorType.Failure);

            if (project.MinimumContribution.HasValue && request.Amount < project.MinimumContribution.Value)
                return new Error("Low_Contribution", $"Minimum contribution for this project is {project.MinimumContribution.Value}", ErrorType.Validation);

            if (project.FundingModel != FundingModel.Reward && project.CurrentAmount + request.Amount > project.TargetAmount)
                return new Error("Over_Funded", "This investment exceeds the remaining target amount.", ErrorType.Validation);

            return null;
        }

        private async Task<Result<Investment>> CreateInvestmentForFundingModelAsync(CreateInvestmentCommand request, Project project, Investor investor, Domain.Entities.Wallet wallet)
        {
            switch (project.FundingModel)
            {
                case FundingModel.Reward:
                    return await CreateRewardInvestmentAsync(request, project, investor, wallet);
                case FundingModel.Equity:
                    return await CreateEquityInvestmentAsync(request, project, investor, wallet);
                case FundingModel.Mudarabah:
                    return CreateMudarabahInvestment(request, project, investor, wallet);
                default:
                    return Result<Investment>.Failure(new List<Error> { new Error("Unknown_Model", "Unknown Funding Model", ErrorType.Failure) });
            }
        }

        private async Task<Result<Investment>> CreateRewardInvestmentAsync(CreateInvestmentCommand request, Project project, Investor investor, Domain.Entities.Wallet wallet)
        {
            if (!project.RewardConfig?.RewardTiers.Any() ?? true)
                return Result<Investment>.Failure(new List<Error> { new Error("No_RewardTiers", "Project has no Reward tiers", ErrorType.NotFound) });

            var rewardTier = request.RewardTierId != null
                ? await _unitOfWork.ProjectRepository.GetRewardTierForUpdateAsync(project.Id, request.RewardTierId.Value)
                : await _unitOfWork.ProjectRepository.GetFirstRewardTierForUpdateAsync(project.Id);

            if (rewardTier is null)
                return Result<Investment>.Failure(new List<Error> { new Error("RewardTier_NotFound", "RewardTier not found", ErrorType.NotFound) });

            if (rewardTier.MaxBackers.HasValue)
            {
                var currentBackers = await _unitOfWork.ProjectRepository
                    .GetRewardTierBackersCountAsync(project.Id, rewardTier.Id);
                if (currentBackers >= rewardTier.MaxBackers.Value)
                    return Result<Investment>.Failure(new List<Error> { new Error("RewardTier_Full", "This RewardTier has reached its max backers limit", ErrorType.Failure) });
            }

            var amountToDeduct = rewardTier.Amount;
            if (wallet.Balance < amountToDeduct)
                return Result<Investment>.Failure(new List<Error> { new Error("Low_Balance", "Insufficient wallet balance", ErrorType.Validation) });

            var investment = new Investment
            {
                Investor = investor,
                Project = project,
                Amount = amountToDeduct,
                RewardTier = rewardTier
            };
            project.CurrentAmount += amountToDeduct;
            return Result<Investment>.Success(investment);
        }

        private async Task<Result<Investment>> CreateEquityInvestmentAsync(CreateInvestmentCommand request, Project project, Investor investor, Domain.Entities.Wallet wallet)
        {
            if (project.EquityConfig is null)
                return Result<Investment>.Failure(new List<Error> { new Error("Invalid_Config", "Project has no Equity configuration", ErrorType.Failure) });

            if (wallet.Balance < request.Amount)
                return Result<Investment>.Failure(new List<Error> { new Error("Low_Balance", "Insufficient wallet balance", ErrorType.Validation) });

            var amountToDeduct = request.Amount;
            decimal calculatedPercentage = (request.Amount / project.TargetAmount) * project.EquityConfig.EquityPercentage;
            var investment = new Investment
            {
                Investor = investor,
                Project = project,
                Amount = amountToDeduct
            };
            var investorShare = new InvestorShare
            {
                Investor = investor,
                Investment = investment,
                EquityConfig = project.EquityConfig,
                AmountInvested = amountToDeduct,
                SharesPercentage = calculatedPercentage
            };

            await _unitOfWork.InvestorShareRepository.AddAsync(investorShare);
            investor.InvestorShares.Add(investorShare);
            project.CurrentAmount += amountToDeduct;
            return Result<Investment>.Success(investment);
        }

        private static Result<Investment> CreateMudarabahInvestment(CreateInvestmentCommand request, Project project, Investor investor, Domain.Entities.Wallet wallet)
        {
            if (project.MudarabahConfig is null)
                return Result<Investment>.Failure(new List<Error> { new Error("Invalid_Config", "Project has no Mudarabah configuration", ErrorType.Failure) });

            if (wallet.Balance < request.Amount)
                return Result<Investment>.Failure(new List<Error> { new Error("Low_Balance", "Insufficient wallet balance", ErrorType.Validation) });

            var amountToDeduct = request.Amount;
            var investment = new Investment
            {
                Investor = investor,
                Project = project,
                Amount = amountToDeduct
            };
            project.CurrentAmount += amountToDeduct;
            return Result<Investment>.Success(investment);
        }

        private async Task FundAndSaveInvestmentAsync(Investment investment, Domain.Entities.Wallet wallet, SystemWallet escrowWallet)
        {
            var project = investment.Project;
            var amountToDeduct = investment.Amount;
            wallet.Withdraw(amountToDeduct);

            var walletTransaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amountToDeduct,
                Type = TransactionType.Investment,
                Status = Domain.Enums.TransactionStatus.Completed,
            };

            escrowWallet.Credit(amountToDeduct);
            var systemWalletTransaction = new SystemWalletTransaction
            {
                SystemWalletId = escrowWallet.Id,
                ProjectId = project.Id,
                InvestmentId = investment.Id,
                Amount = amountToDeduct,
                Type = SystemTransactionType.EscrowHold,
                Notes = $"Escrow hold for investment in project {project.Id}"
            };
            await _unitOfWork.SystemWalletRepository.AddTransactionAsync(systemWalletTransaction);

            if (project.CurrentAmount >= project.TargetAmount)
            {
                project.ProjectStatus = ProjectStatus.FundingClosed;
                await _unitOfWork.ProjectRepository.UpdateAsync(project);
            }

            await _unitOfWork.WalletRepository.AddTransactionAsync(walletTransaction);
            await _unitOfWork.InvestorRepository.UpdateAsync(investment.Investor);
            await _unitOfWork.InvestmentRepository.AddAsync(investment);
            await _unitOfWork.SaveAsync();
        }
    }
}
