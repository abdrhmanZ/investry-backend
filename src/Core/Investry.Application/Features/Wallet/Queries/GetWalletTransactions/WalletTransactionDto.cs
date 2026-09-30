namespace Investry.Application.Features.Wallet.Queries.GetWalletTransactions
{
    public record WalletTransactionDto(
        Guid Id,
        decimal Amount,
        string Type,
        string Status,
        string? StripeSessionId,
        DateTime CreatedAt
    );
}
