using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface IRewardTierRepository : IRepository<RewardTier>
    {
        Task DeleteTiersByConfigIdAsync(Guid rewardConfigId);
    }
}
