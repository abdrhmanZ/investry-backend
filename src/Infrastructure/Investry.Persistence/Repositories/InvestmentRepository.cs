using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class InvestmentRepository : Repository<Investment>, IInvestmentRepository
    {
        private readonly InvestryDbContext _dbContext;

        public InvestmentRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(Guid investorId, Guid projectId)
        {
            return await _dbContext.Investments
                .AnyAsync(i => i.InvestorId == investorId && i.ProjectId == projectId);
        }

        public async Task<Dictionary<Guid, int>> GetBackersCountByTierIdsAsync(IEnumerable<Guid> tierIds)
        {
            return await _dbContext.Investments
                .Where(i => i.RewardTierId.HasValue && tierIds.Contains(i.RewardTierId.Value))
                .GroupBy(i => i.RewardTierId.Value)
                .Select(g => new { TierId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TierId, x => x.Count);
        }

        public async Task<IEnumerable<Investment>> GetByProjectIdAsync(Guid projectId)
        {
            return await _dbContext.Investments
                .Where(i => i.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<bool> HasInvestmentsAsync(Guid projectId)
        {
            return await _dbContext.Investments.AnyAsync(i => i.ProjectId == projectId);
        }

        public async Task<IReadOnlyList<Investment>> GetInvestmentsByInvestorIdAsync(Guid investorId)
        {
            return await _dbContext.Investments
                .Where(i => i.InvestorId == investorId)
                .Include(i => i.Project)
                    .ThenInclude(p => p.Media)
                .Include(i => i.Project)
                    .ThenInclude(p => p.Categories)
                .Include(i => i.Project)
                    .ThenInclude(p => p.MudarabahConfig)
                .Include(i => i.RewardTier)
                .Include(i => i.Investor)
                    .ThenInclude(inv => inv.InvestorShares)
                .Include(i => i.Investor)
                    .ThenInclude(inv => inv.ProfitAllocations)
                .Include(i => i.Investor)
                    .ThenInclude(inv => inv.CapitalReturns)
                .OrderByDescending(i => i.CreatedAt)
                .AsSplitQuery()
                .ToListAsync();
        }
    }
}
