using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Models.Kyc;
using Investry.Application.Services.Helpers;
using Investry.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Investry.Application.Features.KycVerifications.Commands.HandleKycWebhook
{
    public class HandleKycWebhookCommandHandler : IRequestHandler<HandleKycWebhookCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _secret;
        private readonly DiditSettings _settings;
        public HandleKycWebhookCommandHandler(IUnitOfWork unitOfWork, IOptions<DiditSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _settings = settings.Value;
            _secret = _settings.WebhookSecret;
        }
        public async Task<Result<Unit>> Handle(HandleKycWebhookCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Webhook received: SessionId={request.SessionId}, Status={request.Status}, Signature={request.Signature}");

            if (!DiditSignatureHelper.Verify(request.RawJson, request.Signature, _secret))
                return Result<Unit>.Failure(new List<Error>
                {
                    new Error("INVALID_SIGNATURE", "Invalid webhook signature", ErrorType.Failure)
                });

            var kyc = await _unitOfWork.UserRepository.GetKycBySessionIdAsync(request.SessionId);
            if (kyc is null)
                return Result<Unit>.Failure(new List<Error>
                {
                    new Error("KYC_NOT_FOUND", "KYC session not found", ErrorType.Failure)
                });

            // ده يمعلم لو عايز انا اتحكم في الويب هوك عشان اعمل ريسيت للكي واي سي واعيد تقديمه تاني او بالبلدي انا اتحكم في الحاله يدويا من ديديت
            //if (kyc.Status == KycStatus.Approved || kyc.Status == KycStatus.Declined)
            //    return Result<Unit>.Failure(new List<Error>
            //    {
            //        new Error("KYC_ALREADY_PROCESSED", "KYC session already processed", ErrorType.Failure)
            //    });

            var newStatus = request.Status.ToLower() switch
            {
                "approved" => KycStatus.Approved,
                "declined" => KycStatus.Declined,
                "not started" => KycStatus.Pending,
                "in review" => KycStatus.InReview,
                "resubmitted" => KycStatus.Resubmitted,
                _ => KycStatus.Pending
            };

            kyc.Status = newStatus;

            if (newStatus == KycStatus.Approved)
                kyc.VerifiedAt = DateTime.UtcNow;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(kyc.UserId);
            if (user is not null)
            {
                await _unitOfWork.UserRepository.UpdateKycStatusAsync(user.Id, newStatus);
            }

            await _unitOfWork.SaveAsync();

            return Result<Unit>.Success(Unit.Value);
        }
    }
}


/*
 public class HandleKycWebhookCommandHandler : IRequestHandler<HandleKycWebhookCommand, Result<Unit>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _secret;
        private readonly DiditSettings _settings;
        public HandleKycWebhookCommandHandler(IUnitOfWork unitOfWork, IOptions<DiditSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _settings = settings.Value;
            _secret = _settings.WebhookSecret;
        }
        public async Task<Result<Unit>> Handle(HandleKycWebhookCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Webhook received: SessionId={request.SessionId}, Status={request.Status}, Signature={request.Signature}");

            if (!DiditSignatureHelper.Verify(request.RawJson, request.Signature, _secret))
                return Result<Unit>.Failure("Invalid webhook signature");

            var kyc = await _unitOfWork.UserRepository.GetKycBySessionIdAsync(request.SessionId);
            if (kyc is null)
                return Result<Unit>.Failure("KYC session not found");

            if (kyc.Status == KycStatus.Approved || kyc.Status == KycStatus.Rejected)
                return Result<Unit>.Failure("KYC session already processed");

            var newStatus = request.Status.ToLower() switch
            {
                "approved" => KycStatus.Approved,
                "rejected" => KycStatus.Rejected,
                _ => KycStatus.UnderReview
            };

            kyc.Status = newStatus;

            if (newStatus == KycStatus.Approved)
                kyc.VerifiedAt = DateTime.UtcNow;

            if (newStatus == KycStatus.Approved)
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(kyc.UserId);

                if (user is not null)
                    await _unitOfWork.UserRepository.UpdateKycStatusAsync(user.Id, newStatus);
                Console.WriteLine($"User Staus: {user.KycStatus}");
            }

            await _unitOfWork.SaveAsync();

            return Result<Unit>.Success(Unit.Value);
        }
    }
 */
