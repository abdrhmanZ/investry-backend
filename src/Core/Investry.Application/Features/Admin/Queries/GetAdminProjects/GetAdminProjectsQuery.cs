using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminProjects
{
    public record GetAdminProjectsQuery(
        string? Status,
        string? Search
    ) : IRequest<Result<IReadOnlyList<AdminProjectDto>>>;
}
