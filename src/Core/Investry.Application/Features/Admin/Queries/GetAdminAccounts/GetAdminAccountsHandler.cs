using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminAccounts
{
    public class GetAdminAccountsHandler
        : IRequestHandler<GetAdminAccountsQuery, Result<List<AdminAccountDto>>>
    {
        private readonly IIdentityService _identityService;

        public GetAdminAccountsHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<List<AdminAccountDto>>> Handle(
            GetAdminAccountsQuery request, CancellationToken cancellationToken)
        {
            var admins = await _identityService.GetAllAdminsAsync();
            return Result<List<AdminAccountDto>>.Success(admins);
        }
    }
}
