using Investry.Application.Common;
using Investry.Application.Features.Projects.Commands.CreateProject;
using Investry.Application.Models.Media;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.UpdateProject
{
    public record UpdateProjectCommand : IRequest<Result<bool>>
    {
        public Guid ProjectId { get; set; }
        public string UserId { get; set; }

        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? Location { get; set; }

        public decimal? TargetAmount { get; set; }
        public decimal? MinimumContribution { get; set; }
        public int? DurationInDays { get; set; }

        public List<Guid>? CategoryIds { get; set; }

        public List<RewardTierDto>? RewardTiers { get; set; }

        public decimal? EquityPercentage { get; set; }

        public decimal? InvestorsProfitSharePercentage { get; set; }
        public int? MudarabahDurationInMonths { get; set; }
        public ProfitDistributionFrequency? ProfitDistributionFrequency { get; set; }

        public List<FileDto>? NewMediaFiles { get; set; }
        public List<string>? DeletedMediaPublicIds { get; set; }
        public string? NewCoverImagePublicId { get; set; }
    }
}
