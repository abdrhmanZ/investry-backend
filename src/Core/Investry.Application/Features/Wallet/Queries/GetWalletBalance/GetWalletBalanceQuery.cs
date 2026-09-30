using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Wallet.Queries.GetWalletBalance
{
    public record GetWalletBalanceQuery(string UserId) : IRequest<Result<WalletBalanceDto>>;
}
