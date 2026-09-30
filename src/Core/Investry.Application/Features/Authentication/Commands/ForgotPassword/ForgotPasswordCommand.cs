using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result<string>>;
}
