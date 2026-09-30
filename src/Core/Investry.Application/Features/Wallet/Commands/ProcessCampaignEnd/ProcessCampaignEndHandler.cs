using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Models.Email;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;
using System.Transactions;

namespace Investry.Application.Features.Wallet.Commands.ProcessCampaignEnd
{
    public class ProcessCampaignEndHandler : IRequestHandler<ProcessCampaignEndCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public ProcessCampaignEndHandler(IUnitOfWork unitOfWork, IIdentityService identityService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _emailService = emailService;
        }
        public async Task<Result<bool>> Handle(ProcessCampaignEndCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository
            .GetProjectWithInvestmentDataAsync(request.ProjectId);
            if (project is null)
                return Result<bool>.Failure(new List<Error>
            {
                new Error("Project.NotFound", "Project not found.", ErrorType.NotFound)
            });

            var user = await _identityService.GetUserByIdAsync(project.Founder.UserId);

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

            if (project.ProjectStatus != ProjectStatus.FundingClosed)
                return Result<bool>.Failure(new List<Error>
            {
                new Error("Project.InvalidStatus",
                    "Only FundingClosed projects can be processed.", ErrorType.Validation)
            });

            var escrowWallet = await _unitOfWork.SystemWalletRepository
            .GetByTypeAsync(SystemWalletType.Escrow);
            if (escrowWallet is null)
                return Result<bool>.Failure(new List<Error>
            {
                new Error("Escrow.NotFound", "Escrow wallet not found.", ErrorType.NotFound)
            });

            var platformWallet = await _unitOfWork.SystemWalletRepository
                .GetByTypeAsync(SystemWalletType.Platform);
            if (platformWallet is null)
                return Result<bool>.Failure(new List<Error>
            {
                new Error("Platform.NotFound", "Platform wallet not found.", ErrorType.NotFound)
            });

            bool goalAchieved = project.CurrentAmount >= project.TargetAmount;
            bool isKeepItAll = project.FundingPolicy == FundingPolicy.KeepItAll;
            bool shouldRelease = goalAchieved || isKeepItAll;

            if (shouldRelease)
                await ReleaseFundsToFounder(project, escrowWallet, platformWallet, user);
            else
                await RefundInvestors(project, escrowWallet);

            //project.ProjectStatus = shouldRelease
            //? ProjectStatus.Successful
            //: ProjectStatus.Failed;

            await _unitOfWork.ProjectRepository.UpdateAsync(project);
            await _unitOfWork.SaveAsync();

            scope.Complete();

            return Result<bool>.Success(true);
        }

        #region Helper Methods
        private async Task ReleaseFundsToFounder(Project project, SystemWallet escrowWallet, SystemWallet platformWallet, UserDto user)
        {
            var founder = await _unitOfWork.FounderRepository
                .GetByIdAsync(project.FounderId);
            if (founder is null)
                throw new Exception("Founder not found.");

            var founderWallet = await _unitOfWork.WalletRepository
                .GetByFounderUserIdAsync(founder.UserId);
            if (founderWallet is null)
                throw new Exception("Founder wallet not found.");

            var totalEscrow = project.Investments.Sum(i => i.Amount);
            var platformFee = Math.Round(totalEscrow * 0.025m, 2); // 2.5% عند النجاح بس
            var founderAmount = totalEscrow - platformFee;

            // خصم كامل من Escrow
            escrowWallet.Debit(totalEscrow);

            await _unitOfWork.SystemWalletRepository.AddTransactionAsync(new SystemWalletTransaction
            {
                SystemWalletId = escrowWallet.Id,
                ProjectId = project.Id,
                Amount = totalEscrow,
                Type = SystemTransactionType.EscrowRelease,
                Notes = $"Escrow released for project {project.Id}"
            });

            // حط الـ fee في Platform
            platformWallet.Credit(platformFee);

            await _unitOfWork.SystemWalletRepository.AddTransactionAsync(new SystemWalletTransaction
            {
                SystemWalletId = platformWallet.Id,
                ProjectId = project.Id,
                Amount = platformFee,
                Type = SystemTransactionType.PlatformFeeCredit,
                Notes = $"Platform fee 2.5% for project {project.Id}"
            });

            // حط الباقي في Founder Wallet
            founderWallet.Deposit(founderAmount);

            await _unitOfWork.WalletRepository.AddTransactionAsync(new WalletTransaction
            {
                WalletId = founderWallet.Id,
                Amount = founderAmount,
                Type = TransactionType.Deposit,
                Status = Domain.Enums.TransactionStatus.Completed
            });

            project.ProjectStatus = ProjectStatus.Successful;

            var email = new Email
            {
                To = user.Email,
                Subject = "🎉 Congratulations! Your Campaign Was Funded Successfully",
                Body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color:#f4f4f4; padding:20px;'>
                        <div style='max-width:600px; margin:auto; background:#ffffff; padding:20px; border-radius:10px;'>
                            
                            <h2 style='color:#2c3e50;'>🎉 Congratulations, {user.FirstName + " " + user.LastName}!</h2>
                            
                            <p style='font-size:16px; color:#333;'>
                                We’re excited to share some great news!
                            </p>
                    
                            <p style='font-size:16px; color:#333;'>
                                Your campaign <strong>""{project.Title}""</strong> has successfully reached its funding goal. 🚀
                            </p>
                    
                            <p style='font-size:16px; color:#333;'>
                                Thanks to your hard work and the amazing support from your backers, the funds have now been released to your account.
                            </p>
                    
                            <p style='font-size:16px; color:#333;'>
                                You’re now one step closer to bringing your idea to life — and we can’t wait to see what you accomplish next!
                            </p>
                    
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
        }

        private async Task RefundInvestors(Project project, SystemWallet escrowWallet)
        {
            foreach (var investment in project.Investments)
            {
                var investorWallet = await _unitOfWork.WalletRepository
                    .GetByInvestorUserIdAsync(investment.Investor.UserId);
                if (investorWallet is null)
                    throw new Exception(
                        $"Wallet not found for investor {investment.Investor.UserId}");

                var refundAmount = investment.Amount; // 100% كامل

                escrowWallet.Debit(refundAmount);

                await _unitOfWork.SystemWalletRepository.AddTransactionAsync(new SystemWalletTransaction
                {
                    SystemWalletId = escrowWallet.Id,
                    ProjectId = project.Id,
                    InvestmentId = investment.Id,
                    Amount = refundAmount,
                    Type = SystemTransactionType.EscrowRefund,
                    Notes = $"Full refund to investor {investment.Investor.UserId}"
                });

                investorWallet.Deposit(refundAmount);

                await _unitOfWork.WalletRepository.AddTransactionAsync(new WalletTransaction
                {
                    WalletId = investorWallet.Id,
                    Amount = refundAmount,
                    Type = TransactionType.Refund,
                    Status = Domain.Enums.TransactionStatus.Completed
                });

                project.ProjectStatus = ProjectStatus.Failed;
            }
        }
        #endregion
    }
}
