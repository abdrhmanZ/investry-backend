using Investry.Application.Common;
using Investry.Application.Features.Admin.Queries.GetAdminAccounts;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.CreateAdmin
{
    public record CreateAdminCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password
    ) : IRequest<Result<AdminAccountDto>>;
}
