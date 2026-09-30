using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface IProfitDistributionRepository : IRepository<ProfitDistribution>
    {
        Task<bool> ExistsForPeriodAsync(Guid projectId, DateTime periodStart, DateTime periodEnd);
        Task<ProfitDistribution?> GetByPeriodAsync(Guid projectId, DateTime periodStart, DateTime periodEnd);
    }
}
