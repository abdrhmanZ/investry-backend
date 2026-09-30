namespace Investry.Application.Features.ProfitDistributions.Commands.DistributeProfit
{
    public record DistributeProfitRequest(
        Guid ProjectId,
        DateTime PeriodStart,
        DateTime PeriodEnd,
        decimal NetProfit
    );
}
