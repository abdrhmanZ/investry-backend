using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RevokeToken
{
    public record RevokeTokenCommand(string Token) : IRequest<Result<bool>>;
}
