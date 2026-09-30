using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Models.Identity;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RegisterFounder
{
    public class RegisterFounderCommandHandler : IRequestHandler<RegisterFounderCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public RegisterFounderCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(RegisterFounderCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request, UserRole.Founder.ToString());
        }
    }
}
