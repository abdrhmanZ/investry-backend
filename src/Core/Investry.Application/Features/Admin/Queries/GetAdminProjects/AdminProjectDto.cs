namespace Investry.Application.Features.Admin.Queries.GetAdminProjects
{
    public class AdminProjectDto
    {
        // Project Info
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string FundingModel { get; set; }
        public decimal TargetAmount { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Location { get; set; }
        public decimal? MinimumContribution { get; set; }
        public int CampaignDurationInDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProjectStatus { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }

        // Funding Model Specific
        public decimal? EquityPercentage { get; set; }
        public decimal? InvestorsProfitSharePercentage { get; set; }

        // Founder Info (populated from Identity DB in Handler)
        public Guid FounderId { get; set; }
        public string FounderName { get; set; } = string.Empty;
        public string FounderEmail { get; set; } = string.Empty;
    }
}
