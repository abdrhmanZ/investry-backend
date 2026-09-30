using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Users.Commands.UpdateProfile
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;

        public UpdateProfileHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await new UpdateProfileValidator().ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new Error(
                    e.PropertyName,
                    e.ErrorMessage,
                    ErrorType.Validation))
                    .ToList();

                return Result<bool>.Failure(errors);
            }
            var result = await _identityService.UpdateProfileAsync(request);
            return result;
        }
    }
}
