using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.BanUser
{
    public record BanUserCommand(
        string UserId,
        int DurationInDays,
        string Reason
    ) : IRequest<Result<BanUserResponse>>;
}
