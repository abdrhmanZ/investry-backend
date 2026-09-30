using Investry.Application.Common;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.Login
{
    public record LoginCommand : AuthRequest ,IRequest<Result<AuthResponse>>;
}
