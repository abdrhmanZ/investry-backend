using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class RewardTierRepository : Repository<RewardTier>, IRewardTierRepository
    {
        private readonly InvestryDbContext _dbContext;
        public RewardTierRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteTiersByConfigIdAsync(Guid rewardConfigId)
        {
            var tiersToDelete = await _dbContext.RewardTiers
                .Where(rt => rt.RewardConfigId == rewardConfigId)
                .ToListAsync();

            if (tiersToDelete.Any())
            {
                _dbContext.RewardTiers.RemoveRange(tiersToDelete);
            }
        }
    }
}
