using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid FounderId { get; set; }
        public Founder Founder { get; set; }

        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        public FundingModel FundingModel { get; set; }
        public FundingPolicy FundingPolicy { get; set; } = FundingPolicy.AllOrNothing;

        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; } = 0;
        public decimal? MinimumContribution { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? Location { get; set; }
        public string? PromotionalVideoURL { get; set; }

        public ProjectStatus ProjectStatus { get; set; }
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        public ICollection<ProjectMedia> Media { get; set; } = new List<ProjectMedia>();

        // Navigation 
        public RewardConfig? RewardConfig { get; set; }
        public EquityConfig? EquityConfig { get; set; }
        public MudarabahConfig? MudarabahConfig { get; set; }
    }
}
