using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RegisterFounder
{
    public class RegisterInvestorCommandHandler : IRequestHandler<RegisterInvestorCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public RegisterInvestorCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<string>> Handle(RegisterInvestorCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(request, UserRole.Investor.ToString());
        }
    }
}
