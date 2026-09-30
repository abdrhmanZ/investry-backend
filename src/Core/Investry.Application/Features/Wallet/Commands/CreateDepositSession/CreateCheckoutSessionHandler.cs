using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Wallet.Commands.CreateDepositSession
{
    public class CreateCheckoutSessionHandler : IRequestHandler<CreateCheckoutSessionCommand, Result<CheckoutResponse>>
    {
        private readonly IPaymentService _paymentService;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCheckoutSessionHandler(IPaymentService paymentService, IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }
        public async Task<Result<CheckoutResponse>> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0 || decimal.Round(request.Amount, 2) != request.Amount)
                return Result<CheckoutResponse>.Failure(new List<Error> { new Error("Amount.Invalid", "Deposit amount must be positive and have at most two decimal places.", ErrorType.Validation) });

            var user = await _identityService.GetUserByIdAsync(request.UserId);
            if (user is null)
                return Result<CheckoutResponse>.Failure(new List<Error> { new Error("User.NotFound", "User not found", ErrorType.NotFound) });

            if (user.KycStatus != KycStatus.Approved)
                return Result<CheckoutResponse>.Failure(new List<Error> { new Error("User.KycNotApproved", "User KYC is not approved. Please complete KYC verification to proceed.", ErrorType.Validation) });

            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync(request.UserId);
            if (wallet is null)
                return Result<CheckoutResponse>.Failure(new List<Error> { new Error("Wallet.NotFound", "Wallet not found", ErrorType.NotFound) });

            if (request.Amount > 999_999.99m) 
                return Result<CheckoutResponse>.Failure(new List<Error> { new Error("Amount.TooHigh", "The amount exceeds the maximum allowed by Stripe ($999,999.99).", ErrorType.Validation) });

            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = request.Amount,
                Type = TransactionType.Deposit,
                Status = TransactionStatus.Pending
            };

            var response = await _paymentService.CreateCheckoutSessionAsync(
            amount: request.Amount,
            userId: request.UserId,
            metadata: new()
            {
                ["transactionId"] = transaction.Id.ToString(),
                ["userId"] = request.UserId
            }
            );

            transaction.SessionId = response.SessionId;

            await _unitOfWork.WalletRepository.AddTransactionAsync(transaction);
            await _unitOfWork.SaveAsync();

            return Result<CheckoutResponse>.Success(response);
        }
    }
}
