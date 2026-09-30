using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string userId, string Token) : IRequest<Result<string>>;
}
