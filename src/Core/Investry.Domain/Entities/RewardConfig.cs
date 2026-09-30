namespace Investry.Domain.Entities
{
    public class RewardConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<RewardTier> RewardTiers { get; set; } = new List<RewardTier>();
    }
}
