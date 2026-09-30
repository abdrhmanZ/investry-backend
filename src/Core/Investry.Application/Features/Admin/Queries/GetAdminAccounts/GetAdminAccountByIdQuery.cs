using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminAccounts
{
    public record GetAdminAccountByIdQuery(string AdminId)
        : IRequest<Result<AdminAccountDto>>;
}
