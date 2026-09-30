using Investry.Application.Common;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.RegisterFounder
{
    public record RegisterInvestorCommand : RegistrationRequest
        , IRequest<Result<string>>;
}
