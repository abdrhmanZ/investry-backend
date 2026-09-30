using Investry.Application.Common;
using Investry.Application.Common.Models;
using MediatR;

namespace Investry.Application.Features.Wallet.Queries.GetWalletTransactions
{
    public record GetWalletTransactionsQuery(string UserId, int Page = 1, int PageSize = 10) : IRequest<Result<PagedResult<WalletTransactionDto>>>;
}
