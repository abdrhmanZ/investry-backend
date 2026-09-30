namespace Investry.Application.Features.Investments.Queries.GetMyInvestments
{
    public class InvestorInvestmentDto
    {
        // ── Investment Data ──
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime InvestmentDate { get; set; }

        // ── Project Data ──
        public Guid ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjectCoverImageUrl { get; set; }
        public string ProjectCategory { get; set; } = string.Empty;
        public string FundingModel { get; set; } = string.Empty;
        public decimal ProjectTargetAmount { get; set; }
        public decimal ProjectCurrentAmount { get; set; }
        public int FundingProgress => ProjectTargetAmount > 0
            ? (int)((ProjectCurrentAmount / ProjectTargetAmount) * 100)
            : 0;
        public DateTime ProjectEndDate { get; set; }

        // ── Reward-Specific (null if not Reward) ──
        public string? RewardTierTitle { get; set; }
        public string? RewardDescription { get; set; }
        public decimal? RewardTierAmount { get; set; }

        // ── Equity-Specific (null if not Equity) ──
        public decimal? EquityPercentageOwned { get; set; }

        // ── Mudarabah-Specific (null if not Mudarabah) ──
        public decimal? ProfitSharePercentage { get; set; }
        public DateTime? NextPayoutDate { get; set; }
        public decimal? TotalProfitReceived { get; set; }
        public decimal? CapitalReturned { get; set; }
    }
}
