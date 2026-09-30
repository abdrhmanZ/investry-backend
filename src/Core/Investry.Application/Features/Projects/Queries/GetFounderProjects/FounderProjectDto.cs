namespace Investry.Application.Features.Projects.Queries.GetFounderProjects
{
    public class FounderProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }

        public string FundingModel { get; set; }

        public int NumberOfInvestors { get; set; }

        public int FundingProgressPercentage => TargetAmount > 0 ? (int)((CurrentAmount / TargetAmount) * 100) : 0;

        public string ProjectStatus { get; set; }
        public DateTime EndDate { get; set; }

        public string? CoverImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
