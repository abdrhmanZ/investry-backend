using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LogoutAsync(request.RefreshToken);
        }
    }
}
