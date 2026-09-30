using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminAccounts
{
    public class GetAdminAccountByIdHandler
        : IRequestHandler<GetAdminAccountByIdQuery, Result<AdminAccountDto>>
    {
        private readonly IIdentityService _identityService;

        public GetAdminAccountByIdHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AdminAccountDto>> Handle(
            GetAdminAccountByIdQuery request, CancellationToken cancellationToken)
        {
            var admin = await _identityService.GetAdminByIdAsync(request.AdminId);

            if (admin is null)
                return Result<AdminAccountDto>.Failure(new List<Error>
                {
                    new("Admin.NotFound", "Admin not found", ErrorType.NotFound)
                });

            return Result<AdminAccountDto>.Success(admin);
        }
    }
}
