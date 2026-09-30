namespace Investry.Application.Features.Wallet.Queries.GetWalletBalance
{
    public record WalletBalanceDto(decimal Balance, string Currency = "EGP");
}
