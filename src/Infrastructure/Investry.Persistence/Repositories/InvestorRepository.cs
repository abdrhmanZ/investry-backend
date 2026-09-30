using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class InvestorRepository : Repository<Investor>, IInvestorRepository
    {
        private readonly InvestryDbContext _dbContext;

        public InvestorRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Investor?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Investors
                .Include(i => i.Investments)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }

        public async Task<Investor?> GetByIdAsync(Guid investorId)
        {
            return await _dbContext.Investors
                .FirstOrDefaultAsync(i => i.Id == investorId);
        }

        public async Task<Dictionary<string, (int InvestmentCount, decimal TotalInvested)>> GetInvestorStatsByUserIdsAsync(List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return new Dictionary<string, (int, decimal)>();

            return await _dbContext.Investors
                .AsNoTracking()
                .Where(i => userIds.Contains(i.UserId))
                .Select(i => new
                {
                    i.UserId,
                    InvestmentCount = i.Investments.Count,
                    TotalInvested = i.Investments.Sum(inv => inv.Amount)
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => (x.InvestmentCount, x.TotalInvested));
        }
    }
}
