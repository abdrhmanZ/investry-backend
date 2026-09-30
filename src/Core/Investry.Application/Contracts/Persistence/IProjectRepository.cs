using Investry.Application.Features.Admin.Queries.GetAdminProjects;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using Investry.Application.Features.Projects.Queries.GetFounderProjects;
using Investry.Application.Features.Projects.Queries.GetProjectDetails;
using Investry.Domain.Entities;
using Investry.Domain.Enums;

namespace Investry.Application.Contracts.Persistence
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetByTitleAndFounderIdAsync(string title, Guid founderId);
        Task<Project?> GetProjectWithInvestmentDataAsync(Guid projectId);
        public Task<int> GetRewardTierBackersCountAsync(Guid projectId, Guid rewardTierId);
        public Task<RewardTier?> GetRewardTierForUpdateAsync(Guid projectId, Guid rewardTierId);
        public Task<RewardTier?> GetFirstRewardTierForUpdateAsync(Guid projectId);
        // 
        Task<IReadOnlyList<FounderProjectDto>> GetProjectsByFounderIdAsync(Guid founderId);
        Task<IReadOnlyList<ProjectSummaryDto>> GetAllProjectsSummaryAsync();
        Task<IReadOnlyList<AdminProjectDto>> GetAdminProjectsAsync(ProjectStatus? status);
        Task<ProjectDetailsDto?> GetProjectDetailsAsync(Guid projectId);
        Task<Project?> GetByIdAsyncWithIgnoreFilter(Guid id);
        Task<Project?> GetProjectWithDependenciesForDeletionAsync(Guid projectId);
        Task<Project?> GetProjectWithDependenciesForUpdateAsync(Guid projectId);
        Task<(IEnumerable<Project> Items, int TotalCount)> GetEndedCampaignsForAdminAsync(int page, int pageSize);
    }
}
