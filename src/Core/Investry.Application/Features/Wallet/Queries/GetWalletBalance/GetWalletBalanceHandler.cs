using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using MediatR;

namespace Investry.Application.Features.Wallet.Queries.GetWalletBalance
{
    public class GetWalletBalanceHandler : IRequestHandler<GetWalletBalanceQuery, Result<WalletBalanceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWalletBalanceHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<WalletBalanceDto>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(request.UserId);
            if (wallet is null)
                return Result<WalletBalanceDto>.Failure(new List<Error> { new Error("Wallet.NotFound", "Wallet not found", ErrorType.NotFound) });

            return Result<WalletBalanceDto>.Success(new WalletBalanceDto(wallet.Balance));
        }
    }
}
