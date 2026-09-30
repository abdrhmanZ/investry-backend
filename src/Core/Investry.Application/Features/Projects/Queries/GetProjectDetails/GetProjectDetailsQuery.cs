using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.GetProjectDetails
{
    public record GetProjectDetailsQuery(Guid ProjectId) : IRequest<Result<ProjectDetailsDto>>;
}
