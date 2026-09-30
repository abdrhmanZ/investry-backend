using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    // هنسجل فيه كل مرة بيتم فيها توزيع الارباح
    public class ProfitDistribution
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid MudarabahConfigId { get; set; }
        public MudarabahConfig MudarabahConfig { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public decimal NetProfit { get; set; }

        // ده نصيب المستثمرين من الربح في الفترة دي
        public decimal InvestorsPoolProfit { get; set; }
        // ده نصيب المؤسس من الربح في الفترة دي
        public decimal FounderProfit { get; set; }

        public DistributionStatus DistributionStatus { get; set; }

        public DateTime? DistributedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public ICollection<InvestorProfitAllocation> InvestorProfitAllocations { get; set; } = new List<InvestorProfitAllocation>();
    }
}
