using Investry.Application.Common;
using Investry.Application.Models.Identity;
using MediatR;

namespace Investry.Application.Features.Authentication.Commands.ResetPassword
{
    public record ResetPasswordCommand : ResetPasswordRequest 
        ,IRequest<Result<string>>;
}
