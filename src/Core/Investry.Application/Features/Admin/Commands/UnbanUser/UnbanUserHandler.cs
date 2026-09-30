using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.UnbanUser
{
    public class UnbanUserHandler
        : IRequestHandler<UnbanUserCommand, Result<UnbanUserResponse>>
    {
        private readonly IIdentityService _identityService;

        public UnbanUserHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<UnbanUserResponse>> Handle(
            UnbanUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.UnbanUserAsync(request.UserId);

            if (result.IsFailure)
                return Result<UnbanUserResponse>.Failure(result.Errors);

            return Result<UnbanUserResponse>.Success(new UnbanUserResponse
            {
                UserId = request.UserId,
                Status = "Active",
                Message = "User account has been reactivated"
            });
        }
    }
}
