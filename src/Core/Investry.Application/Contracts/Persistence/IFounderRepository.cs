using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface IFounderRepository : IRepository<Founder>
    {
        Task<Founder?> GetByUserIdAsync(string userId);
        Task<Founder?> GetByIdAsync(Guid founderId);
        Task<List<Founder>> GetByFounderIdsAsync(List<Guid> founderIds);
        Task<Dictionary<string, (int ProjectCount, decimal TotalRaised)>> GetFounderStatsByUserIdsAsync(List<string> userIds);
    }
}
