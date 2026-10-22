using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Projects.Queries.GetAllProjects;

namespace Investry.Application.Features.Projects.Queries
{
    internal static class ProjectSummaryEnricher
    {
        public static async Task PopulateAsync(IEnumerable<ProjectSummaryDto> projects, IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            var founderIds = projects.Select(project => project.FounderId).Distinct().ToList();
            if (founderIds.Count == 0)
                return;

            var founders = await unitOfWork.FounderRepository.GetByFounderIdsAsync(founderIds);
            var userIds = founders.Select(founder => founder.UserId).Distinct().ToList();
            var userNames = await identityService.GetUserNamesByIdsAsync(userIds);
            var foundersById = founders.ToDictionary(founder => founder.Id);
            var now = DateTime.UtcNow;

            foreach (var project in projects)
            {
                if (foundersById.TryGetValue(project.FounderId, out var founder)
                    && userNames.TryGetValue(founder.UserId, out var founderName))
                    project.FounderName = founderName;

                var remainingDays = (project.EndDate - now).TotalDays;
                project.DaysRemaining = remainingDays > 0 ? (int)Math.Ceiling(remainingDays) : 0;
            }
        }
    }
}
