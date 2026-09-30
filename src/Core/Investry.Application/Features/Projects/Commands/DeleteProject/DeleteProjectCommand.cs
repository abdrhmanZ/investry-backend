using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.DeleteProject
{
    public record DeleteProjectCommand(Guid ProjectId, string UserId) : IRequest<Result<bool>>;
}
