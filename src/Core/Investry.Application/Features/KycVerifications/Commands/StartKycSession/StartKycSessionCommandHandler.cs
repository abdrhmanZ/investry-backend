using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.DTOs;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.KycVerification.Commands.StartKycSession
{
    public class StartKycSessionCommandHandler : IRequestHandler<StartKycSessionCommand, Result<StartKycSessionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKycProvider _kycProvider;
        public StartKycSessionCommandHandler(IUnitOfWork unitOfWork, IKycProvider kycProvider)
        {
            _unitOfWork = unitOfWork;
            _kycProvider = kycProvider;
        }
        public async Task<Result<StartKycSessionResponse>> Handle(StartKycSessionCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.UserRepository
                .GetActiveKycForUserAsync(request.UserId);

            if (existing is not null)
            {
                if (existing.Status == KycStatus.Pending)
                {
                    return Result<StartKycSessionResponse>.Success(new StartKycSessionResponse
                    {
                        SessionId = existing.ProviderSessionId,
                        VerificationUrl = existing.VerificationUrl
                    });
                }
                else
                {
                    return Result<StartKycSessionResponse>.Failure(new List<Error>
                    {
                        new Error("KYC_EXISTS", "An active KYC session already exists for this user.", ErrorType.Failure)
                    });
                }
            }

            var response = await _kycProvider.CreateSessionAsync(request.UserId);
            if (response is null)
            {
                return Result<StartKycSessionResponse>.Failure(new List<Error>
                {
                    new Error("KYC_PROVIDER_ERROR", "Failed to create KYC session with the provider.", ErrorType.Failure)
                });
            }

            var kyc = new Investry.Domain.Entities.KycVerification
            {
                UserId = request.UserId,
                ProviderSessionId = response.SessionId,
                VerificationUrl = response.VerificationUrl,
                Status = KycStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserRepository.AddKycVerificationAsync(kyc);
            await _unitOfWork.SaveAsync();

            return Result<StartKycSessionResponse>.Success(response);
        }
    }
}
