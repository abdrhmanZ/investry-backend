using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.RejectProject
{
    public class RejectProjectHandler
        : IRequestHandler<RejectProjectCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public RejectProjectHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Result<string>> Handle(
            RejectProjectCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                return Result<string>.Failure(new List<Error>
                {
                    new("Project.ReasonRequired", "Rejection reason is required.", ErrorType.Validation)
                });

            var project = await _unitOfWork.ProjectRepository.GetAsync(request.ProjectId);

            if (project is null)
                return Result<string>.Failure(new List<Error>
                {
                    new("Project.NotFound", "Project not found.", ErrorType.NotFound)
                });

            if (project.ProjectStatus != ProjectStatus.PendingReview)
                return Result<string>.Failure(new List<Error>
                {
                    new("Project.InvalidStatus",
                        $"Only projects with status '{nameof(ProjectStatus.PendingReview)}' can be rejected. Current status: '{project.ProjectStatus}'.",
                        ErrorType.Validation)
                });

            project.ProjectStatus = ProjectStatus.Rejected;
            project.RejectionReason = request.Reason;
            project.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            await SendRejectionEmailAsync(project.FounderId, project.Title, request.Reason);

            return Result<string>.Success("Project rejected successfully.");
        }

        private async Task SendRejectionEmailAsync(Guid founderId, string projectTitle, string reason)
        {
            try
            {
                var founder = await _unitOfWork.FounderRepository.GetByIdAsync(founderId);
                if (founder is null) return;

                var userInfo = await _identityService.GetUserByIdAsync(founder.UserId);
                if (userInfo is null) return;

                var email = new Models.Email.Email
                {
                    To = userInfo.Email,
                    Subject = "Investry — Your project has been reviewed",
                    Body = $"Dear {userInfo.FirstName} {userInfo.LastName},\n\n"
                         + $"Your project \"{projectTitle}\" has been reviewed and unfortunately was not approved.\n\n"
                         + $"Reason: {reason}\n\n"
                         + "You can update your project and resubmit for review.\n\n"
                         + "Best regards,\nInvestry Team",
                    HtmlBody = $"<p>Dear <strong>{userInfo.FirstName} {userInfo.LastName}</strong>,</p>"
                             + $"<p>Your project <strong>\"{projectTitle}\"</strong> has been reviewed and unfortunately was not approved.</p>"
                             + $"<p><strong>Reason:</strong> {reason}</p>"
                             + "<p>You can update your project and resubmit for review.</p>"
                             + "<br/><p>Best regards,<br/><strong>Investry Team</strong></p>"
                };

                await _emailService.SendAsync(email);
            }
            catch
            {
                // Email failure should not affect the rejection flow
                // The project status has already been saved successfully
            }
        }
    }
}
