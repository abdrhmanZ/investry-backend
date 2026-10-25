using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Investments.Commands.CreateInvestment;
using Investry.Application.Models.Email;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Moq;
using Xunit;

namespace Investry.Tests;

public class InvestmentFlowTests
{
    [Theory]
    [InlineData(FundingModel.Reward, 100, 250, ProjectStatus.FundingClosed, "receive your selected reward")]
    [InlineData(FundingModel.Equity, 200, 200, ProjectStatus.FundingClosed, "own a share in this project")]
    [InlineData(FundingModel.Mudarabah, 100, 100, ProjectStatus.Published, "returns based on the project's performance")]
    public async Task SuccessfulInvestmentMovesTheChargedAmountToEscrowAndConfirmsItsFundingModel(
        FundingModel model, int requestedAmount, int chargedAmount, ProjectStatus expectedStatus, string emailDetails)
    {
        var scenario = new InvestmentScenario(model, 500);

        var response = await scenario.Handler.Handle(scenario.Command(requestedAmount), default);

        Assert.True(response.IsSuccess);
        var investment = Assert.Single(scenario.Investments);
        Assert.Equal(investment.Id, response.Value);
        Assert.Same(scenario.Project, investment.Project);
        Assert.Same(scenario.Investor, investment.Investor);
        Assert.Equal(chargedAmount, investment.Amount);
        Assert.Equal(500 - chargedAmount, scenario.Wallet.Balance);
        Assert.Equal(chargedAmount, scenario.Escrow.Balance);
        Assert.Equal(0, scenario.Platform.Balance);
        Assert.Equal(200 + chargedAmount, scenario.Project.CurrentAmount);
        Assert.Equal(expectedStatus, scenario.Project.ProjectStatus);

        var walletTransaction = Assert.Single(scenario.WalletTransactions);
        Assert.Equal(scenario.Wallet.Id, walletTransaction.WalletId);
        Assert.Equal(chargedAmount, walletTransaction.Amount);
        Assert.Equal(TransactionType.Investment, walletTransaction.Type);
        Assert.Equal(TransactionStatus.Completed, walletTransaction.Status);
        var escrowTransaction = Assert.Single(scenario.EscrowTransactions);
        Assert.Equal(scenario.Escrow.Id, escrowTransaction.SystemWalletId);
        Assert.Equal(scenario.Project.Id, escrowTransaction.ProjectId);
        Assert.Equal(investment.Id, escrowTransaction.InvestmentId);
        Assert.Equal(chargedAmount, escrowTransaction.Amount);
        Assert.Equal(SystemTransactionType.EscrowHold, escrowTransaction.Type);

        Assert.Equal(model == FundingModel.Reward ? scenario.RewardTier : null, investment.RewardTier);
        if (model == FundingModel.Equity)
        {
            var share = Assert.Single(scenario.Shares);
            Assert.Same(share, Assert.Single(scenario.Investor.InvestorShares));
            Assert.Same(investment, share.Investment);
            Assert.Same(scenario.Investor, share.Investor);
            Assert.Same(scenario.Project.EquityConfig, share.EquityConfig);
            Assert.Equal(chargedAmount, share.AmountInvested);
            Assert.Equal(15m, share.SharesPercentage);
        }
        else
        {
            Assert.Empty(scenario.Shares);
            Assert.Empty(scenario.Investor.InvestorShares);
        }

        var email = Assert.Single(scenario.Emails);
        Assert.Equal("investor@example.com", email.To);
        Assert.Contains(scenario.Project.Title, email.Body);
        Assert.Contains($"{chargedAmount} EGP", email.Body);
        Assert.Contains(emailDetails, email.Body);
    }

    [Theory]
    [InlineData(FundingModel.Reward)]
    [InlineData(FundingModel.Equity)]
    [InlineData(FundingModel.Mudarabah)]
    public async Task InsufficientBalanceLeavesFundsAndInvestmentsUnchanged(FundingModel model)
    {
        var scenario = new InvestmentScenario(model, 50);

        var response = await scenario.Handler.Handle(scenario.Command(100), default);

        Assert.True(response.IsFailure);
        Assert.Equal("Low_Balance", response.Errors.Single().Code);
        Assert.Equal(50, scenario.Wallet.Balance);
        AssertNoInvestmentEffects(scenario);
    }

    [Fact]
    public async Task FullRewardTierRejectsAnInvestmentEvenWhenTheWalletCanPay()
    {
        var scenario = new InvestmentScenario(FundingModel.Reward, 500);
        scenario.ProjectRepository.Setup(r => r.GetRewardTierBackersCountAsync(scenario.Project.Id, scenario.RewardTier.Id))
            .ReturnsAsync(scenario.RewardTier.MaxBackers!.Value);

        var response = await scenario.Handler.Handle(scenario.Command(100), default);

        Assert.True(response.IsFailure);
        Assert.Equal("RewardTier_Full", response.Errors.Single().Code);
        Assert.Equal(500, scenario.Wallet.Balance);
        AssertNoInvestmentEffects(scenario);
    }

    private static void AssertNoInvestmentEffects(InvestmentScenario scenario)
    {
        Assert.Equal(0, scenario.Escrow.Balance);
        Assert.Equal(0, scenario.Platform.Balance);
        Assert.Equal(200, scenario.Project.CurrentAmount);
        Assert.Equal(ProjectStatus.Published, scenario.Project.ProjectStatus);
        Assert.Empty(scenario.Investments);
        Assert.Empty(scenario.Shares);
        Assert.Empty(scenario.Investor.InvestorShares);
        Assert.Empty(scenario.WalletTransactions);
        Assert.Empty(scenario.EscrowTransactions);
        Assert.Empty(scenario.Emails);
    }

    private sealed class InvestmentScenario
    {
        public Project Project { get; }
        public Investor Investor { get; } = new() { UserId = "investor" };
        public Wallet Wallet { get; } = new();
        public SystemWallet Escrow { get; } = new() { Type = SystemWalletType.Escrow };
        public SystemWallet Platform { get; } = new() { Type = SystemWalletType.Platform };
        public RewardTier RewardTier { get; } = new() { Amount = 250, MaxBackers = 5 };
        public Mock<IProjectRepository> ProjectRepository { get; } = new();
        public List<Investment> Investments { get; } = new();
        public List<InvestorShare> Shares { get; } = new();
        public List<WalletTransaction> WalletTransactions { get; } = new();
        public List<SystemWalletTransaction> EscrowTransactions { get; } = new();
        public List<Email> Emails { get; } = new();
        public CreateInvestmentHandler Handler { get; }

        public InvestmentScenario(FundingModel model, decimal balance)
        {
            Project = new Project
            {
                Title = "Community solar",
                FundingModel = model,
                ProjectStatus = ProjectStatus.Published,
                TargetAmount = 400,
                CurrentAmount = 200,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                RewardConfig = model == FundingModel.Reward ? new RewardConfig { RewardTiers = new List<RewardTier> { RewardTier } } : null,
                EquityConfig = model == FundingModel.Equity ? new EquityConfig { EquityPercentage = 30 } : null,
                MudarabahConfig = model == FundingModel.Mudarabah ? new MudarabahConfig() : null
            };
            Wallet.Deposit(balance);
            ProjectRepository.Setup(r => r.GetProjectWithInvestmentDataAsync(Project.Id)).ReturnsAsync(Project);
            ProjectRepository.Setup(r => r.GetRewardTierForUpdateAsync(Project.Id, RewardTier.Id)).ReturnsAsync(RewardTier);
            var investors = new Mock<IInvestorRepository>();
            investors.Setup(r => r.GetByUserIdAsync(Investor.UserId)).ReturnsAsync(Investor);
            var wallets = new Mock<IWalletRepository>();
            wallets.Setup(r => r.GetByUserIdAsync(Investor.UserId)).ReturnsAsync(Wallet);
            wallets.Setup(r => r.AddTransactionAsync(It.IsAny<WalletTransaction>()))
                .Callback<WalletTransaction>(WalletTransactions.Add).Returns(Task.CompletedTask);
            var systemWallets = new Mock<ISystemWalletRepository>();
            systemWallets.Setup(r => r.GetByTypeAsync(SystemWalletType.Escrow)).ReturnsAsync(Escrow);
            systemWallets.Setup(r => r.GetByTypeAsync(SystemWalletType.Platform)).ReturnsAsync(Platform);
            systemWallets.Setup(r => r.AddTransactionAsync(It.IsAny<SystemWalletTransaction>()))
                .Callback<SystemWalletTransaction>(EscrowTransactions.Add).Returns(Task.CompletedTask);
            var investments = new Mock<IInvestmentRepository>();
            investments.Setup(r => r.AddAsync(It.IsAny<Investment>()))
                .Callback<Investment>(Investments.Add).ReturnsAsync((Investment investment) => investment);
            var shares = new Mock<IInvestorShareRepository>();
            shares.Setup(r => r.AddAsync(It.IsAny<InvestorShare>()))
                .Callback<InvestorShare>(Shares.Add).ReturnsAsync((InvestorShare share) => share);
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(u => u.ProjectRepository).Returns(ProjectRepository.Object);
            unitOfWork.SetupGet(u => u.InvestorRepository).Returns(investors.Object);
            unitOfWork.SetupGet(u => u.WalletRepository).Returns(wallets.Object);
            unitOfWork.SetupGet(u => u.SystemWalletRepository).Returns(systemWallets.Object);
            unitOfWork.SetupGet(u => u.InvestmentRepository).Returns(investments.Object);
            unitOfWork.SetupGet(u => u.InvestorShareRepository).Returns(shares.Object);
            unitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
            var identity = new Mock<IIdentityService>();
            identity.Setup(i => i.GetUserByIdAsync(Investor.UserId))
                .ReturnsAsync(new UserDto { Email = "investor@example.com", FirstName = "Mona", LastName = "Hassan" });
            var emailService = new Mock<IEmailService>();
            emailService.Setup(e => e.SendAsync(It.IsAny<Email>())).Callback<Email>(Emails.Add).ReturnsAsync(true);
            Handler = new CreateInvestmentHandler(unitOfWork.Object, identity.Object, emailService.Object);
        }

        public CreateInvestmentCommand Command(decimal amount) =>
            new(Project.Id, Investor.UserId, amount, Project.FundingModel == FundingModel.Reward ? RewardTier.Id : null);
    }
}
