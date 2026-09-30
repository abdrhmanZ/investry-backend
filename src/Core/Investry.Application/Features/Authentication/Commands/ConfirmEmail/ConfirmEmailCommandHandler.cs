using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public ConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ConfirmEmailAsync(request.userId, request.Token);
        }
    }
}
