using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface IInvestorRepository : IRepository<Investor>
    {
        Task<Investor?> GetByUserIdAsync(string userId);
        Task<Investor?> GetByIdAsync(Guid investorId);
        Task<Dictionary<string, (int InvestmentCount, decimal TotalInvested)>> GetInvestorStatsByUserIdsAsync(List<string> userIds);
    }
}
