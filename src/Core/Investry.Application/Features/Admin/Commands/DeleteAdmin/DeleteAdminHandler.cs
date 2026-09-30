using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.DeleteAdmin
{
    public class DeleteAdminHandler
        : IRequestHandler<DeleteAdminCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;

        public DeleteAdminHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<string>> Handle(
            DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            if (request.AdminId == request.CurrentAdminId)
                return Result<string>.Failure(new List<Error>
                {
                    new("Admin.SelfDeletion", "You cannot delete your own account.", ErrorType.Validation)
                });

            var result = await _identityService.DeleteAdminAsync(
                request.AdminId, request.CurrentAdminId);

            if (result.IsFailure)
                return Result<string>.Failure(result.Errors);

            return Result<string>.Success("Admin access removed successfully");
        }
    }
}
