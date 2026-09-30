using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IAuthService _authService;
        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.refreshToken);
        }
    }
}
