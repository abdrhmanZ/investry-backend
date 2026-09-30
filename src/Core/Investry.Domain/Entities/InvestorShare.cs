namespace Investry.Domain.Entities
{
    public class InvestorShare
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid InvestorId { get; set; }
        public Investor Investor { get; set; }

        public Guid InvestmentId { get; set; }
        public Investment Investment { get; set; }

        public Guid EquityConfigId { get; set; }
        public EquityConfig EquityConfig { get; set; }

        public decimal AmountInvested { get; set; }
        public decimal SharesPercentage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
