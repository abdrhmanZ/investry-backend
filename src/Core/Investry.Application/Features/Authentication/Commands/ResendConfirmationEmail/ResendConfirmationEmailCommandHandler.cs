using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public ResendConfirmationEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authService.ResendConfirmationEmailAsync(request.email);
        }
    }
}
