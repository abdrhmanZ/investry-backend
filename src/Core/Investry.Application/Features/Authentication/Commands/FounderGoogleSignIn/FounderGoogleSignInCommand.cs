using Investry.Application.Common;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.FounderGoogleSignIn
{
    public record FounderGoogleSignInCommand(string IdToken) : IRequest<Result<AuthResponse>>;
}
