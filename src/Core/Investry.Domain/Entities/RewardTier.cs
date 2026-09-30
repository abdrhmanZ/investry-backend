namespace Investry.Domain.Entities
{
    public class RewardTier
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RewardConfigId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int? MaxBackers { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation 
        public RewardConfig RewardConfig { get; set; }

        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
    }
}
