using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Admin.Queries.GetAdminProjects;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using Investry.Application.Features.Projects.Queries.GetFounderProjects;
using Investry.Application.Features.Projects.Queries.GetProjectDetails;
using Investry.Application.Features.Projects.Queries.SearchProjects;
using Investry.Application.Common.Models;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Investry.Persistence.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly InvestryDbContext _dbContext;
        private static readonly Expression<Func<Project, ProjectSummaryDto>> SummaryProjection = project => new ProjectSummaryDto
        {
            Id = project.Id,
            FounderId = project.FounderId,
            Title = project.Title,
            ShortDescription = project.ShortDescription,
            FundingModel = project.FundingModel.ToString(),
            TargetAmount = project.TargetAmount,
            CurrentAmount = project.CurrentAmount,
            ProjectStatus = project.ProjectStatus.ToString(),
            EndDate = project.EndDate,
            CoverImageUrl = project.Media.FirstOrDefault(media => media.IsCover).MediaUrl,
            Category = project.Categories.FirstOrDefault().Name
        };

        public ProjectRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Project?> GetByTitleAndFounderIdAsync(string title, Guid founderId)
        {
            return await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.Title == title && p.FounderId == founderId);
        }

        public async Task<Project?> GetProjectWithInvestmentDataAsync(Guid projectId)
        {
            return await _dbContext.Projects
                .Include(p=>p.Founder)
                .Include(p => p.Investments) 
                     .ThenInclude(i => i.Investor)
                .Include(p => p.RewardConfig)
                    .ThenInclude(rc => rc.RewardTiers)
                .Include(p => p.EquityConfig)
                .Include(p => p.MudarabahConfig)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<int> GetRewardTierBackersCountAsync(Guid projectId, Guid rewardTierId)
        {
            return await _dbContext.Investments
                .CountAsync(i => i.ProjectId == projectId && i.RewardTierId == rewardTierId);
        }

        public async Task<RewardTier?> GetRewardTierForUpdateAsync(Guid projectId, Guid rewardTierId)
        {
            return await _dbContext.Projects
                .Where(p => p.Id == projectId)
                .SelectMany(p => p.RewardConfig.RewardTiers)
                .Where(rt => rt.Id == rewardTierId)
                .AsTracking() // مهم جداً للـ Row Lock أثناء TransactionScope
                .FirstOrDefaultAsync();
        }

        public async Task<RewardTier?> GetFirstRewardTierForUpdateAsync(Guid projectId)
        {
            return await _dbContext.Projects
                .Where(p => p.Id == projectId)
                .SelectMany(p => p.RewardConfig.RewardTiers)
                .AsTracking() // مهم جداً
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<FounderProjectDto>> GetProjectsByFounderIdAsync(Guid founderId)
        {
            return await _dbContext.Projects
                .Where(p => p.FounderId == founderId)
                .AsNoTracking()
                .OrderByDescending(p=>p.CreatedAt)
                .Select(p=>new FounderProjectDto
                { 
                    Id = p.Id,
                    Title = p.Title,
                    FundingModel = p.FundingModel.ToString(),
                    TargetAmount = p.TargetAmount,
                    CurrentAmount = p.CurrentAmount,
                    NumberOfInvestors = p.Investments.Select(i => i.InvestorId).Distinct().Count(),
                    ProjectStatus = p.ProjectStatus.ToString(),
                    EndDate = p.EndDate,

                    CoverImageUrl = p.Media.FirstOrDefault(m => m.IsCover).MediaUrl,
                    Category = p.Categories.FirstOrDefault().Name
                })
                .ToListAsync();
        }
        public async Task<IReadOnlyList<ProjectSummaryDto>> GetAllProjectsSummaryAsync()
        {
            return await _dbContext.Projects
                .AsNoTracking()
                .Where(project => project.ProjectStatus == ProjectStatus.Published)
                .OrderByDescending(project => project.CreatedAt)
                .Select(SummaryProjection)
                .ToListAsync();
        }

        public async Task<PagedResult<ProjectSummaryDto>> SearchProjectsAsync(SearchProjectsQuery request, CancellationToken cancellationToken)
        {
            var query = GetFilteredPublishedProjects(request);
            var totalCount = await query.CountAsync(cancellationToken);
            var projects = await query
                .OrderByDescending(project => project.CreatedAt)
                .ThenBy(project => project.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(SummaryProjection)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProjectSummaryDto>(projects, totalCount, request.Page, request.PageSize);
        }

        private IQueryable<Project> GetFilteredPublishedProjects(SearchProjectsQuery request)
        {
            var query = _dbContext.Projects.AsNoTracking()
                .Where(project => project.ProjectStatus == ProjectStatus.Published);
            var search = request.Search?.Trim();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(project => project.Title.Contains(search) || project.ShortDescription.Contains(search));
            if (request.CategoryId.HasValue)
                query = query.Where(project => project.Categories.Any(category => category.Id == request.CategoryId.Value));
            if (request.FundingModel.HasValue)
                query = query.Where(project => project.FundingModel == request.FundingModel.Value);

            return query;
        }

        public async Task<IReadOnlyList<AdminProjectDto>> GetAdminProjectsAsync(ProjectStatus? status)
        {
            var query = _dbContext.Projects
                .AsNoTracking();

            if (status.HasValue)
                query = query.Where(p => p.ProjectStatus == status.Value);

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new AdminProjectDto
                {
                    Id = p.Id,
                    FounderId = p.FounderId,
                    Title = p.Title,
                    ShortDescription = p.ShortDescription,
                    LongDescription = p.LongDescription,
                    FundingModel = p.FundingModel.ToString(),
                    TargetAmount = p.TargetAmount,
                    CoverImageUrl = p.Media.FirstOrDefault(m => m.IsCover).MediaUrl,
                    Location = p.Location,
                    MinimumContribution = p.MinimumContribution,
                    CampaignDurationInDays = (int)(p.EndDate - p.StartDate).TotalDays,
                    CreatedAt = p.CreatedAt,
                    ProjectStatus = p.ProjectStatus.ToString(),
                    Category = p.Categories.FirstOrDefault().Name ?? "Uncategorized",
                    RejectionReason = p.RejectionReason,
                    EquityPercentage = p.FundingModel == FundingModel.Equity
                        ? p.EquityConfig.EquityPercentage : null,
                    InvestorsProfitSharePercentage = p.FundingModel == FundingModel.Mudarabah
                        ? p.MudarabahConfig.InvestorsProfitSharePercentage : null
                })
                .ToListAsync();
        }

        public async Task<ProjectDetailsDto?> GetProjectDetailsAsync(Guid projectId)
        {
            return await _dbContext.Projects
            .Where(p => p.Id == projectId)
            .AsNoTracking()
            //.OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDetailsDto
            {
                Id = p.Id,
                FounderId = p.FounderId,
                Title = p.Title,
                ShortDescription = p.ShortDescription,
                LongDescription = p.LongDescription,

                TargetAmount = p.TargetAmount,
                CurrentAmount = p.CurrentAmount,
                MinimumContribution = p.MinimumContribution,
                NumberOfInvestors = p.Investments.Select(i => i.InvestorId).Distinct().Count(),
                ProjectStatus = p.ProjectStatus.ToString(),
                Location = p.Location,

                CampaignDurationInDays = (int)(p.EndDate - p.StartDate).TotalDays,
                DaysRemaining = (int)(p.EndDate - DateTime.UtcNow).TotalDays,

                PromotionalVideoURL = p.PromotionalVideoURL,
                MediaGallery = p.Media.Where(m => m.Type == MediaType.Image).Select(m => new MediaDto { Url = m.MediaUrl, PublicId = m.PublicId, Type = m.Type.ToString(), IsCover = m.IsCover }).ToList(),
                MediaDocument = p.Media.Where(m => m.Type == MediaType.Document).Select(m => new MediaDto { Url = m.MediaUrl, PublicId = m.PublicId, Type = m.Type.ToString(), IsCover = m.IsCover }).ToList(),

                Category = p.Categories.Select(c => c.Name).FirstOrDefault() ?? "Uncategorized",

                FundingModel = p.FundingModel.ToString(),
                RewardTiers = p.FundingModel == FundingModel.Reward ? p.RewardConfig.RewardTiers.Select(rt => new RewardTierDetailsDto
                {
                    Id = rt.Id,
                    Title = rt.Title,
                    Description = rt.Description,
                    Amount = rt.Amount,
                    MaxBackers = rt.MaxBackers,
                    CurrentBackers = rt.Investments.Count(),
                }).ToList() : null,
                EquityDetails = p.FundingModel == FundingModel.Equity ? new EquityDetailsDto { EquityPercentageOffered = p.EquityConfig.EquityPercentage } : null,
                MudarabahDetails = p.FundingModel == FundingModel.Mudarabah ? new MudarabahDetailsDto
                {
                    InvestorsProfitSharePercentage = p.MudarabahConfig.InvestorsProfitSharePercentage,
                    ProfitDistributionFrequency = p.MudarabahConfig.ProfitDistributionFrequency.ToString(),
                    ContractDurationInMonths = p.MudarabahConfig.DurationInMonths
                } : null
            })
            .FirstOrDefaultAsync();
        }

        public async Task<Project?> GetByIdAsyncWithIgnoreFilter(Guid id)
        {
            return await _dbContext.Projects
                                   .IgnoreQueryFilters()
                                   .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project?> GetProjectWithDependenciesForDeletionAsync(Guid projectId)
        {
            return await _dbContext.Projects
                .IgnoreQueryFilters()
                .Include(p => p.RewardConfig)
                    .ThenInclude(rc => rc.RewardTiers)
                .Include(p => p.EquityConfig)
                    .ThenInclude(ec => ec.InvestorShares)
                .Include(p => p.MudarabahConfig)
                    .ThenInclude(mc => mc.ProfitDistributions)
                        .ThenInclude(pd => pd.InvestorProfitAllocations)
                .Include(p => p.MudarabahConfig)
                    .ThenInclude(mc => mc.CapitalReturns)
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<Project?> GetProjectWithDependenciesForUpdateAsync(Guid projectId)
        {
            return await _dbContext.Projects
                .Include(p => p.Categories)
                .Include(p => p.Media)
                .Include(p => p.RewardConfig).ThenInclude(rc => rc.RewardTiers)
                .Include(p => p.EquityConfig)
                .Include(p => p.MudarabahConfig)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

        public async Task<(IEnumerable<Project> Items, int TotalCount)> GetEndedCampaignsForAdminAsync(int page, int pageSize)
        {
            var query = _dbContext.Projects
                .Include(p => p.Founder)
                .Include(p => p.Investments)
                .Where(p =>
                    p.EndDate <= DateTime.UtcNow ||
                    (p.ProjectStatus == ProjectStatus.FundingClosed ||      // منتهية ولسه مش اتعالجت
                     p.ProjectStatus == ProjectStatus.Successful ||         // اتعالجت وتم الـ Release
                     p.ProjectStatus == ProjectStatus.Failed))              // منتهية ولسه مش اتعالجت أو اتعالجت وتم الـ Release أو منتهية وتمت ولكن لم تحقق الهدف
                .OrderByDescending(p => p.EndDate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
