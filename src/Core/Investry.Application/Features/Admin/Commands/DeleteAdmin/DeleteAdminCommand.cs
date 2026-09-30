using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.DeleteAdmin
{
    public record DeleteAdminCommand(
        string AdminId,
        string CurrentAdminId
    ) : IRequest<Result<string>>;
}
