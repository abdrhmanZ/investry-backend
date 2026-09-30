using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class ProfitDistributionRepository : Repository<ProfitDistribution>, IProfitDistributionRepository
    {
        private readonly InvestryDbContext _dbContext;

        public ProfitDistributionRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsForPeriodAsync(Guid projectId, DateTime periodStart, DateTime periodEnd)
        {
            return await _dbContext.ProfitDistributions
                .Include(pd => pd.MudarabahConfig)
                .AnyAsync(pd =>
                    pd.MudarabahConfig.ProjectId == projectId &&
                    pd.PeriodStart == periodStart &&
                    pd.PeriodEnd == periodEnd);
        }

        public async Task<ProfitDistribution?> GetByPeriodAsync(Guid projectId, DateTime periodStart, DateTime periodEnd)
        {
            return await _dbContext.ProfitDistributions
                .Include(pd => pd.MudarabahConfig)
                .Include(pd => pd.InvestorProfitAllocations)
                .FirstOrDefaultAsync(pd =>
                    pd.MudarabahConfig.ProjectId == projectId &&
                    pd.PeriodStart == periodStart &&
                    pd.PeriodEnd == periodEnd);
        }
    }
}
