using Investry.Application.Common;
using Investry.Application.Common.Models;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminUsers
{
    public record GetAdminUsersQuery(
        int Page,
        int PageSize,
        string? Role,
        string? Status,
        string? Search
    ) : IRequest<Result<PagedResult<AdminUserDto>>>;
}
