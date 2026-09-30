using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface IInvestmentRepository : IRepository<Investment>
    {
        Task<bool> ExistsAsync(Guid investorId, Guid projectId);
        public Task<Dictionary<Guid, int>> GetBackersCountByTierIdsAsync(IEnumerable<Guid> tierIds);
        Task<IEnumerable<Investment>> GetByProjectIdAsync(Guid projectId);
        Task<bool> HasInvestmentsAsync(Guid projectId);
        Task<IReadOnlyList<Investment>> GetInvestmentsByInvestorIdAsync(Guid investorId);
    }
}
