using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;
using System.Transactions;
using TransactionStatus = Investry.Domain.Enums.TransactionStatus;

namespace Investry.Application.Features.Wallet.Commands.ConfirmDeposit
{
    public class ConfirmDepositHandler : IRequestHandler<ConfirmDepositCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmDepositHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(ConfirmDepositCommand request, CancellationToken cancellationToken)
        {
            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.Serializable },
                TransactionScopeAsyncFlowOption.Enabled);

            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(request.UserId);
            if (wallet is null)
                return Result<bool>.Failure(new List<Error> { new Error("Wallet.NotFound", "Wallet not found for the user.", ErrorType.NotFound) });

            var transaction = await _unitOfWork.WalletRepository.GetTransactionBySessionIdAsync(request.SessionId);
            if (transaction is null)
                return Result<bool>.Failure(new List<Error> { new Error("Transaction.NotFound", "No transaction found for the provided session ID.", ErrorType.NotFound) });

            if (transaction.WalletId != wallet.Id || transaction.Type != TransactionType.Deposit ||
                transaction.Amount != request.Amount || request.Amount <= 0)
                return Result<bool>.Failure(new List<Error> { new Error("Transaction.InvalidDeposit", "The deposit does not match the pending transaction.", ErrorType.Validation) });

            if (transaction.Status == TransactionStatus.Completed)
                return Result<bool>.Success(true);

            if (transaction.Status != TransactionStatus.Pending)
                return Result<bool>.Failure(new List<Error> { new Error("Transaction.InvalidStatus", "Only pending deposits can be completed.", ErrorType.Conflict) });

            wallet.Deposit(request.Amount);
            transaction.Status = TransactionStatus.Completed;

            await _unitOfWork.SaveAsync();
            scope.Complete();

            return Result<bool>.Success(true);
        }
    }
}
