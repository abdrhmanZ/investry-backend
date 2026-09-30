using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.ActivateMudarabah
{
    public record ActivateMudarabahCommand(Guid ProjectId, string UserId) : IRequest<Result<bool>>;
}
