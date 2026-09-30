using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.UnbanUser
{
    public record UnbanUserCommand(string UserId) : IRequest<Result<UnbanUserResponse>>;
}
