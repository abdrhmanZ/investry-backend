namespace Investry.Application.Features.Admin.Queries.GetEndedCampaigns
{
    public record EndedCampaignDto(
        Guid ProjectId,
        string ProjectTitle,
        string FounderName,
        string FounderEmail,
        decimal TargetAmount,
        decimal CollectedAmount,
        int FundingProgressPercentage,
        int InvestorsCount,
        DateTime EndDate,
        decimal EscrowAmount,
        string ReleaseStatus  // Released / PendingRelease
    );
}
