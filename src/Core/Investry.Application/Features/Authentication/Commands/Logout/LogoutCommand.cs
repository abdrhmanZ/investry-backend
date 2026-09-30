using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<Result<string>>;
}
