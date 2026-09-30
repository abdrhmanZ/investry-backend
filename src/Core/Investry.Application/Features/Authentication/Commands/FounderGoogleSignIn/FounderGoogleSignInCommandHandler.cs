using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Models.Identity;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.FounderGoogleSignIn
{
    public class FounderGoogleSignInCommandHandler : IRequestHandler<FounderGoogleSignInCommand, Result<AuthResponse>>
    {
        private readonly IAuthService _authService;
        public FounderGoogleSignInCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<AuthResponse>> Handle(FounderGoogleSignInCommand request, CancellationToken cancellationToken)
        {
            return await _authService.SignInWithGoogleAsync(request.IdToken, UserRole.Founder.ToString());
        }
    }
}
