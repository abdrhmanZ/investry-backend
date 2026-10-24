using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.GetProjectDetails
{
    public record GetProjectDetailsQuery(Guid ProjectId) : IRequest<Result<ProjectDetailsDto>>
    {
        public string? UserId { get; init; }
        public bool IsAdministrator { get; init; }
    }
}
