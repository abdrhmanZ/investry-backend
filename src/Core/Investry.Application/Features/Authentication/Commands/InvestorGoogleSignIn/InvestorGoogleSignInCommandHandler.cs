using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Models.Identity;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.InvestorGoogleSignIn
{
    public class InvestorGoogleSignInCommandHandler : IRequestHandler<InvestorGoogleSignInCommand, Result<AuthResponse>>
    {
        private readonly IAuthService _authService;
        public InvestorGoogleSignInCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<AuthResponse>> Handle(InvestorGoogleSignInCommand request, CancellationToken cancellationToken)
        {
            return await _authService.SignInWithGoogleAsync(request.IdToken, UserRole.Investor.ToString());
        }
    }
}
