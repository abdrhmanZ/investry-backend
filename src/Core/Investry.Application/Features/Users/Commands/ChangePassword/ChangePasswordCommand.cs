using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Users.Commands.ChangePassword
{
    public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword) : IRequest<Result<bool>>;
}
