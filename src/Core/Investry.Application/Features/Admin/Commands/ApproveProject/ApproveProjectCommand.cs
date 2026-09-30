using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.ApproveProject
{
    public record ApproveProjectCommand(Guid ProjectId) : IRequest<Result<string>>;
}
