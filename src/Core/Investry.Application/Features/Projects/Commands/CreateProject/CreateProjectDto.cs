using Investry.Domain.Enums;

namespace Investry.Application.Features.Projects.Commands.CreateProject
{
    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        public FundingModel FundingModel { get; set; }

        public decimal TargetAmount { get; set; }

        public decimal? MinimumContribution { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Guid> CategoryIds { get; set; } = new List<Guid>();

        // Reward
        public List<RewardTierDto>? RewardTiers { get; set; }
        // Equity
        public decimal? EquityPercentage { get; set; }
        // Mudarabah
        public decimal? InvestorsProfitSharePercentage { get; set; }
        public int? DurationInMonths { get; set; }
        public ProfitDistributionFrequency? ProfitDistributionFrequency { get; set; }
    }
}
