using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Models.Email;
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

            var alreadyInvested = await _unitOfWork.InvestmentRepository.ExistsAsync(investor.Id, project.Id);

            if (alreadyInvested)
                return Result<Guid>.Failure(new List<Error> { new Error("Duplicate_Investment", "You already invested in this project.", ErrorType.Failure) });

            if (project.MinimumContribution.HasValue && request.Amount < project.MinimumContribution.Value)
                return Result<Guid>.Failure(new List<Error>
                {
                    new Error("Low_Contribution", $"Minimum contribution for this project is {project.MinimumContribution.Value}", ErrorType.Validation)
                });

            if (project.FundingModel != FundingModel.Reward && project.CurrentAmount + request.Amount > project.TargetAmount)
                return Result<Guid>.Failure(new List<Error> { new Error("Over_Funded", "This investment exceeds the remaining target amount.", ErrorType.Validation) });

            Investment investment;
            decimal amountToDeduct;

            switch (project.FundingModel)
            {
                case FundingModel.Reward:
                    if (!project.RewardConfig?.RewardTiers.Any() ?? true)
                        return Result<Guid>.Failure(new List<Error> { new Error("No_RewardTiers", "Project has no Reward tiers", ErrorType.NotFound) });

                    var rewardTier = request.RewardTierId != null
                        ? await _unitOfWork.ProjectRepository.GetRewardTierForUpdateAsync(project.Id, request.RewardTierId.Value)
                        : await _unitOfWork.ProjectRepository.GetFirstRewardTierForUpdateAsync(project.Id);

                    if (rewardTier is null)
                        return Result<Guid>.Failure(new List<Error> { new Error("RewardTier_NotFound", "RewardTier not found", ErrorType.NotFound) });
                   
                    if (rewardTier.MaxBackers.HasValue)
                    {
                        var currentBackers = await _unitOfWork.ProjectRepository
                            .GetRewardTierBackersCountAsync(project.Id, rewardTier.Id);
                        if (currentBackers >= rewardTier.MaxBackers.Value)
                            return Result<Guid>.Failure(new List<Error> { new Error("RewardTier_Full", "This RewardTier has reached its max backers limit", ErrorType.Failure) });
                    }

                    amountToDeduct = rewardTier.Amount;

                    if (wallet.Balance < amountToDeduct)
                        return Result<Guid>.Failure(new List<Error> { new Error("Low_Balance", "Insufficient wallet balance", ErrorType.Validation) });

                    investment = new Investment
                    {
                        Investor = investor,
                        Project = project,
                        Amount = amountToDeduct,
                        RewardTier = rewardTier
                    };
                    project.CurrentAmount += amountToDeduct;
                    break;

                // Equity logic Investment
                case FundingModel.Equity:
                    if (project.EquityConfig is null)
                        return Result<Guid>.Failure(new List<Error> { new Error("Invalid_Config", "Project has no Equity configuration", ErrorType.Failure) });

                    if (wallet.Balance < request.Amount)
                        return Result<Guid>.Failure(new List<Error> { new Error("Low_Balance", "Insufficient wallet balance", ErrorType.Validation) });

                    amountToDeduct = request.Amount;

                    decimal calculatedPercentage = (request.Amount / project.TargetAmount) * project.EquityConfig.EquityPercentage;

                    investment = new Investment
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
                    break;

                // Mudarabah logic Investment
                case FundingModel.Mudarabah:
                    if (project.MudarabahConfig is null)
                        return Result<Guid>.Failure(new List<Error> { new Error("Invalid_Config", "Project has no Mudarabah configuration", ErrorType.Failure) });

                    if (wallet.Balance < request.Amount)
                        return Result<Guid>.Failure(new List<Error> { new Error("Low_Balance", "Insufficient wallet balance", ErrorType.Validation) });

                    amountToDeduct = request.Amount;

                    investment = new Investment
                    {
                        Investor = investor,
                        Project = project,
                        Amount = amountToDeduct
                    };

                    project.CurrentAmount += amountToDeduct;
                    break;

                default:
                    return Result<Guid>.Failure(new List<Error> { new Error("Unknown_Model", "Unknown Funding Model", ErrorType.Failure) });
            }

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
            await _unitOfWork.InvestorRepository.UpdateAsync(investor);
            await _unitOfWork.InvestmentRepository.AddAsync(investment);
            await _unitOfWork.SaveAsync();

            string extraDetails = "";

            switch (project.FundingModel)
            {
                case FundingModel.Reward:
                    extraDetails = $@"
                                        <p style='font-size:16px; color:#333;'>
                                            🎁 You will receive your selected reward once the campaign is completed.
                                        </p>";
                    break;

                case FundingModel.Equity:
                    extraDetails = $@"
                                        <p style='font-size:16px; color:#333;'>
                                            📊 You now own a share in this project based on your investment.
                                        </p>";
                    break;

                case FundingModel.Mudarabah:
                    extraDetails = $@"
                                        <p style='font-size:16px; color:#333;'>
                                            🤝 Your investment will generate returns based on the project's performance.
                                        </p>";
                    break;
            }

            var email = new Email
            {
                To = user.Email,
                Subject = "💰 Investment Successful",
                Body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                        </head>
                        <body style='font-family: Arial, sans-serif; background-color:#f4f4f4; padding:20px;'>
                        
                            <div style='max-width:600px; margin:auto; background:#ffffff; padding:25px; border-radius:12px;'>
                        
                                <h2 style='color:#2c3e50;'>💰 Investment Confirmed, {user?.FirstName + " " + user?.LastName ?? "Investor"}!</h2>
                        
                                <p style='font-size:16px; color:#333;'>
                                    Your investment has been successfully processed.
                                </p>
                        
                                <p style='font-size:16px; color:#333;'>
                                    <strong>Project:</strong> ""{project.Title}""
                                </p>
                        
                                <p style='font-size:16px; color:#333;'>
                                    <strong>Amount:</strong> {amountToDeduct} EGP
                                </p>
                        
                                <p style='font-size:16px; color:#333;'>
                                    <strong>Date:</strong> {DateTime.UtcNow:yyyy-MM-dd}
                                </p>
                        
                                {extraDetails}
                        
                                <p style='font-size:16px; color:#333;'>
                                    The funds have been securely placed in escrow and will be handled according to the campaign outcome.
                                </p>
                        
                                <!-- CTA Button -->
                                <div style='text-align:center; margin:30px 0;'>
                                    <a href='https://investry-project.vercel.app/investor/investments' 
                                       style='background-color:#2c3e50; color:#ffffff; padding:12px 20px; text-decoration:none; border-radius:6px; font-size:16px;'>
                                       View Your Investment
                                    </a>
                                </div>
                        
                                <hr style='margin:20px 0;'>
                        
                                <p style='font-size:14px; color:#777;'>
                                    If you have any questions, feel free to contact our support team anytime.
                                </p>
                        
                                <p style='font-size:14px; color:#2c3e50; font-weight:bold;'>
                                    — InvesTry Team
                                </p>
                        
                            </div>
                        
                        </body>
                        </html>
                        "
            };
            await _emailService.SendAsync(email);

            scope.Complete();

            return Result<Guid>.Success(investment.Id);
        }
    }
}
