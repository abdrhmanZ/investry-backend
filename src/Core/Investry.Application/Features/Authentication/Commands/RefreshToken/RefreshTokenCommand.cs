using Investry.Application.Common;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<Result<AuthResponse>>;
}
