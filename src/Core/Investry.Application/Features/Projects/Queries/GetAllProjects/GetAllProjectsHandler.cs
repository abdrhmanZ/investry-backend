using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsHandler : IRequestHandler<GetAllProjectsQuery, Result<IReadOnlyList<ProjectSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public GetAllProjectsHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<IReadOnlyList<ProjectSummaryDto>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _unitOfWork.ProjectRepository.GetAllProjectsSummaryAsync();
            await ProjectSummaryEnricher.PopulateAsync(projects, _unitOfWork, _identityService);
            return Result<IReadOnlyList<ProjectSummaryDto>>.Success(projects);
        }
    }
}
