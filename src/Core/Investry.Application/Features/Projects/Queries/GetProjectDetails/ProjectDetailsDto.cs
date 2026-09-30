using Investry.Domain.Entities;
using Investry.Domain.Enums;

namespace Investry.Application.Features.Projects.Queries.GetProjectDetails
{
    public class ProjectDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public Guid FounderId { get; set; }
        public string FounderName { get; set; }

        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal? MinimumContribution { get; set; }
        public int NumberOfInvestors { get; set; }
        public int FundingProgressPercentage => TargetAmount > 0 ? (int)((CurrentAmount / TargetAmount) * 100) : 0;
        public int CampaignDurationInDays { get; set; }
        public int DaysRemaining { get; set; }
        public string ProjectStatus { get; set; }
        public string? Location { get; set; }

        public string? PromotionalVideoURL { get; set; }
        public List<MediaDto> MediaGallery { get; set; } = new List<MediaDto>();
        public List<MediaDto> MediaDocument { get; set; } = new List<MediaDto>();

        public string Category { get; set; } = string.Empty;

        public string FundingModel { get; set; }
        public List<RewardTierDetailsDto>? RewardTiers { get; set; } // DTO من الرد السابق
        public EquityDetailsDto? EquityDetails { get; set; }
        public MudarabahDetailsDto? MudarabahDetails { get; set; }
    }
}
