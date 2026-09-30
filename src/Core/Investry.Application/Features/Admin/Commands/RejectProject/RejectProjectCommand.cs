using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Commands.RejectProject
{
    public record RejectProjectCommand(Guid ProjectId, string Reason) : IRequest<Result<string>>;
}
