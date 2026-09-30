using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.BanUser
{
    public class BanUserHandler
        : IRequestHandler<BanUserCommand, Result<BanUserResponse>>
    {
        private static readonly HashSet<int> AllowedDurations = new() { 1, 3, 7, 30, -1 };
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public BanUserHandler(IIdentityService identityService, IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result<BanUserResponse>> Handle(
            BanUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reason) || request.Reason.Length < 5)
                return Result<BanUserResponse>.Failure(new List<Error>
                {
                    new("Ban.ReasonRequired", "Ban reason is required (minimum 5 characters).", ErrorType.Validation)
                });

            if (!AllowedDurations.Contains(request.DurationInDays))
                return Result<BanUserResponse>.Failure(new List<Error>
                {
                    new("Ban.InvalidDuration", "Duration must be 1, 3, 7, 30, or -1 (permanent).", ErrorType.Validation)
                });

            DateTime? banExpiry = request.DurationInDays == -1
                ? null
                : DateTime.UtcNow.AddDays(request.DurationInDays);

            var result = await _identityService.BanUserAsync(
                request.UserId, request.Reason, banExpiry);

            if (result.IsFailure)
                return Result<BanUserResponse>.Failure(result.Errors);

            await SendBanEmailAsync(request.UserId, request.Reason, request.DurationInDays, banExpiry);

            return Result<BanUserResponse>.Success(new BanUserResponse
            {
                UserId = request.UserId,
                Status = "Banned",
                BanReason = request.Reason,
                BanExpiry = banExpiry,
                Message = "User has been banned successfully"
            });
        }

        private async Task SendBanEmailAsync(string userId, string reason, int durationInDays, DateTime? banExpiry)
        {
            try
            {
                var userInfo = await _identityService.GetUserByIdAsync(userId);
                if (userInfo is null) return;

                var durationText = durationInDays == -1
                    ? "permanently"
                    : $"for {FormatDuration(durationInDays)}";

                var expiryText = banExpiry.HasValue
                    ? $"Your ban will expire on {banExpiry.Value:MMMM dd, yyyy}."
                    : "This ban is permanent.";

                var email = new Models.Email.Email
                {
                    To = userInfo.Email,
                    Subject = "Investry — Your account has been suspended",
                    Body = $"Dear {userInfo.FirstName} {userInfo.LastName},\n\n"
                         + $"Your Investry account has been suspended {durationText}.\n\n"
                         + $"Reason: {reason}\n\n"
                         + $"{expiryText}\n\n"
                         + "If you believe this was a mistake, please contact our support team.\n\n"
                         + "Best regards,\nInvestry Team",
                    HtmlBody = $"<p>Dear <strong>{userInfo.FirstName} {userInfo.LastName}</strong>,</p>"
                             + $"<p>Your Investry account has been <strong>suspended {durationText}</strong>.</p>"
                             + $"<p><strong>Reason:</strong> {reason}</p>"
                             + $"<p>{expiryText}</p>"
                             + "<p>If you believe this was a mistake, please contact our support team.</p>"
                             + "<br/><p>Best regards,<br/><strong>Investry Team</strong></p>"
                };

                await _emailService.SendAsync(email);
            }
            catch
            {
                // Email failure should not affect the ban flow
                // The user status has already been updated successfully
            }
        }

        private static string FormatDuration(int days) => days switch
        {
            1 => "1 day",
            3 => "3 days",
            7 => "1 week",
            30 => "1 month",
            _ => $"{days} days"
        };
    }
}
