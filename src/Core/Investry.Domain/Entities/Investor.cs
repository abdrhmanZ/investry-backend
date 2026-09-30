namespace Investry.Domain.Entities
{
    public class Investor
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        public Wallet Wallet { get; set; }

        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
        public ICollection<InvestorShare> InvestorShares { get; set; } = new List<InvestorShare>();

        public ICollection<InvestorProfitAllocation> ProfitAllocations { get; set; } = new List<InvestorProfitAllocation>();
        public ICollection<CapitalReturn> CapitalReturns { get; set; } = new List<CapitalReturn>();
    }
}
