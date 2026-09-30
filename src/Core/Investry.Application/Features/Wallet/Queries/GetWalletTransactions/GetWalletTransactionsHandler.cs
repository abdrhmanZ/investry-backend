using Investry.Application.Common;
using Investry.Application.Common.Models;
using Investry.Application.Contracts.Persistence;
using MediatR;

namespace Investry.Application.Features.Wallet.Queries.GetWalletTransactions
{
    public class GetWalletTransactionsHandler : IRequestHandler<GetWalletTransactionsQuery, Result<PagedResult<WalletTransactionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWalletTransactionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<PagedResult<WalletTransactionDto>>> Handle(GetWalletTransactionsQuery request, CancellationToken cancellationToken)
        {
            if (request.Page <= 0 || request.PageSize <= 0)
                return Result<PagedResult<WalletTransactionDto>>.Failure(new List<Error> { new Error("Invalid_Pagination", "Page and PageSize must be greater than 0.", ErrorType.Validation) });

            var (items, totalCount) = await _unitOfWork.WalletRepository.GetTransactionsByUserIdAsync(request.UserId, request.Page, request.PageSize);
            var dtos = items.Select(t => new WalletTransactionDto(
                Id: t.Id,
                Amount: t.Amount,
                Type: t.Type.ToString(),
                Status: t.Status.ToString(),
                StripeSessionId: t.SessionId,
                CreatedAt: t.CreatedAt
            ));

            return Result<PagedResult<WalletTransactionDto>>.Success(
                new PagedResult<WalletTransactionDto>(
                    Items: dtos,
                    TotalCount: totalCount,
                    Page: request.Page,
                    PageSize: request.PageSize
                ));
        }
    }
}
