using Investry.Application.Common;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.InvestorGoogleSignIn
{
    public record InvestorGoogleSignInCommand(string IdToken) : IRequest<Result<AuthResponse>>;
}
