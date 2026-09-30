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
            if (projects == null)
                return Result<IReadOnlyList<ProjectSummaryDto>>.Failure(new List<Error> { new Error("ProjectsNotFound", "No projects found.", ErrorType.NotFound) });

            var founderIds = projects.Select(p => p.FounderId).Distinct().ToList();

            var founders = await _unitOfWork.FounderRepository.GetByFounderIdsAsync(founderIds);
            var userIds = founders.Select(f => f.UserId).ToList();
            var usersDictionary = await _identityService.GetUserNamesByIdsAsync(userIds);
            foreach (var projectDto in projects)
            {
                var founder = founders.FirstOrDefault(f => f.Id == projectDto.FounderId);
                if (founder != null && usersDictionary.TryGetValue(founder.UserId, out var founderName))
                {
                    projectDto.FounderName = founderName;
                }

                var remainingTimeSpan = projectDto.EndDate - DateTime.UtcNow;
                projectDto.DaysRemaining = remainingTimeSpan.TotalDays > 0 ? (int)Math.Ceiling(remainingTimeSpan.TotalDays) : 0;
            }

            return Result<IReadOnlyList<ProjectSummaryDto>>.Success(projects);
        }
    }
}