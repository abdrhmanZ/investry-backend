using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Features.Admin.Queries.GetAdminAccounts;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.CreateAdmin
{
    public class CreateAdminHandler
        : IRequestHandler<CreateAdminCommand, Result<AdminAccountDto>>
    {
        private readonly IIdentityService _identityService;

        public CreateAdminHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AdminAccountDto>> Handle(
            CreateAdminCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                return Result<AdminAccountDto>.Failure(new List<Error>
                {
                    new("Admin.FirstNameRequired", "First name is required", ErrorType.Validation)
                });

            if (string.IsNullOrWhiteSpace(request.LastName))
                return Result<AdminAccountDto>.Failure(new List<Error>
                {
                    new("Admin.LastNameRequired", "Last name is required", ErrorType.Validation)
                });

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<AdminAccountDto>.Failure(new List<Error>
                {
                    new("Admin.EmailRequired", "Email is required", ErrorType.Validation)
                });

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                return Result<AdminAccountDto>.Failure(new List<Error>
                {
                    new("Admin.PasswordTooShort", "Password must be at least 6 characters", ErrorType.Validation)
                });

            return await _identityService.CreateAdminAsync(
                request.FirstName.Trim(),
                request.LastName.Trim(),
                request.Email.Trim(),
                request.Password);
        }
    }
}
