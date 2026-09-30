using System.ComponentModel.DataAnnotations.Schema;

namespace Investry.Domain.Entities
{
    public class Investment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid InvestorId { get; set; }
        public Investor Investor { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal Amount { get; set; }

        // Reward خاص بال
        public Guid? RewardTierId { get; set; }
        public RewardTier? RewardTier { get; set; }

        // Mudarabah خاص بال 
        // نسبة مساهمة المستثمر من رأس المال 
        [NotMapped]
        public decimal? CapitalRatio => Amount / Project.TargetAmount;
    }
}
