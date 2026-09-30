using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ResendConfirmationEmail
{
    public record ResendConfirmationEmailCommand(string email) : IRequest<Result<string>>;
}
