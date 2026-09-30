using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RevokeToken
{
    public class RevokeTokenCommandHndler : IRequestHandler<RevokeTokenCommand, Result<bool>>
    {
        private readonly IAuthService _authService;
        public RevokeTokenCommandHndler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RevokeTokenAsync(request.Token);
        }
    }
}
