using Investry.Domain.Enums;

namespace Investry.Application.Features.Projects.Queries.GetAllProjects
{
    public class ProjectSummaryDto
    {
        public Guid Id { get; set; }
        public Guid FounderId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }

        public string FundingModel { get; set; }

        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }

        public int FundingProgressPercentage => TargetAmount > 0 ? (int)((CurrentAmount / TargetAmount) * 100) : 0;

        public DateTime EndDate { get; set; } 
        public int DaysRemaining { get; set; }

        public string ProjectStatus { get; set; }
        public string? CoverImageUrl { get; set; }
        //public List<string> CategoryNames { get; set; } = new List<string>();
        public string Category { get; set; } = string.Empty;

        public string FounderName { get; set; }
    }
}
