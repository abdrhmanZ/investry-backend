using Investry.Application.Common;
using Investry.Application.Common.Models;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.SearchProjects
{
    public record SearchProjectsQuery : IRequest<Result<PagedResult<ProjectSummaryDto>>>
    {
        public string? Search { get; init; }
        public Guid? CategoryId { get; init; }
        public FundingModel? FundingModel { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 12;
    }
}
