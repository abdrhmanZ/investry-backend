using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;

        public ChangePasswordHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var validator = new ChangePasswordValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new Error(
                    e.PropertyName,
                    e.ErrorMessage,
                    ErrorType.Validation))
                    .ToList();

                return Result<bool>.Failure(errors);
            }
            var result = await _identityService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
            return result;
        }
    }
}
