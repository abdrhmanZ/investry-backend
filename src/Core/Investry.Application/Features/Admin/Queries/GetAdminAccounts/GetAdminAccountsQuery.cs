using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminAccounts
{
    public record GetAdminAccountsQuery() : IRequest<Result<List<AdminAccountDto>>>;
}
