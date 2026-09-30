using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminUserDetails
{
    public record GetAdminUserDetailsQuery(string UserId)
        : IRequest<Result<Queries.GetAdminUsers.AdminUserDto>>;
}
