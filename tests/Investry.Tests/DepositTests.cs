using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Wallet.Commands.ConfirmDeposit;
using Investry.Application.Features.Wallet.Commands.ProcessWebhook;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Investry.Tests;

public class DepositTests
{
    private readonly Wallet _wallet = new();
    private readonly WalletTransaction _transaction;
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPaymentService> _paymentService = new();

    public DepositTests()
    {
        _transaction = new WalletTransaction
        {
            WalletId = _wallet.Id,
            Amount = 50,
            Type = TransactionType.Deposit,
            Status = TransactionStatus.Pending,
            SessionId = "test-session"
        };
        var repository = new Mock<IWalletRepository>();
        repository.Setup(r => r.GetByUserIdAsync("test-user")).ReturnsAsync(_wallet);
        repository.Setup(r => r.GetTransactionBySessionIdAsync("test-session")).ReturnsAsync(_transaction);
        _unitOfWork.SetupGet(u => u.WalletRepository).Returns(repository.Object);
        _unitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
        _paymentService.Setup(p => p.ParseWebhookEvent("payload", "signature"))
            .Returns(new WebhookResult("checkout.session.completed", "test-session", 50,
                new Dictionary<string, string> { ["userId"] = "test-user" }));
    }

    [Fact]
    public async Task RepeatedPaidWebhookCreditsTheWalletOnce()
    {
        using var provider = Provider();
        var mediator = provider.GetRequiredService<IMediator>();
        var command = new ProcessWebhookCommand("payload", "signature");

        var first = await mediator.Send(command);
        var duplicate = await mediator.Send(command);

        Assert.True(first.IsSuccess);
        Assert.True(duplicate.IsSuccess);
        Assert.Equal(50, _wallet.Balance);
        Assert.Equal(TransactionStatus.Completed, _transaction.Status);
    }

    [Theory]
    [InlineData("wallet")]
    [InlineData("amount")]
    [InlineData("type")]
    [InlineData("status")]
    public async Task MismatchedOrFailedDepositDoesNotCreditTheWallet(string mismatch)
    {
        switch (mismatch)
        {
            case "wallet": _transaction.WalletId = Guid.NewGuid(); break;
            case "amount": _transaction.Amount = 25; break;
            case "type": _transaction.Type = TransactionType.Investment; break;
            case "status": _transaction.Status = TransactionStatus.Failed; break;
        }
        using var provider = Provider();

        var response = await provider.GetRequiredService<IMediator>()
            .Send(new ProcessWebhookCommand("payload", "signature"));

        Assert.True(response.IsFailure);
        Assert.Equal(0, _wallet.Balance);
        Assert.NotEqual(TransactionStatus.Completed, _transaction.Status);
    }

    [Fact]
    public async Task DatabaseFailureIsNotAcknowledgedAsSuccessfulWebhook()
    {
        _unitOfWork.Setup(u => u.SaveAsync()).ThrowsAsync(new InvalidOperationException("Database unavailable"));
        using var provider = Provider();

        await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetRequiredService<IMediator>()
            .Send(new ProcessWebhookCommand("payload", "signature")));
    }

    [Fact]
    public async Task InvalidSignatureDoesNotCreditTheWallet()
    {
        _paymentService.Setup(p => p.ParseWebhookEvent("payload", "signature"))
            .Throws(new ArgumentException("Invalid signature"));
        using var provider = Provider();

        var response = await provider.GetRequiredService<IMediator>()
            .Send(new ProcessWebhookCommand("payload", "signature"));

        Assert.True(response.IsFailure);
        Assert.Equal("Webhook.InvalidSignature", response.Errors.Single().Code);
        Assert.Equal(0, _wallet.Balance);
    }

    private ServiceProvider Provider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<ConfirmDepositHandler>());
        services.AddSingleton(_unitOfWork.Object);
        services.AddSingleton(_paymentService.Object);
        return services.BuildServiceProvider();
    }
}
