using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ResetPasswordAsync(request);
        }
    }
}
