using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class FounderRepository : Repository<Founder>, IFounderRepository
    {
        private readonly InvestryDbContext _dbContext;

        public FounderRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Founder?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Founders
                .Include(f => f.Projects)
                .FirstOrDefaultAsync(f => f.UserId == userId);
        }

        public async Task<Founder?> GetByIdAsync(Guid founderId)
        {
            return await _dbContext.Founders
                .FirstOrDefaultAsync(f => f.Id == founderId);
        }

        public async Task<List<Founder>> GetByFounderIdsAsync(List<Guid> founderIds)
        {
            if (founderIds == null || !founderIds.Any())
            {
                return new List<Founder>();
            }

            return await _dbContext.Founders
                                   .Where(f => founderIds.Contains(f.Id))
                                   .ToListAsync();
        }

        public async Task<Dictionary<string, (int ProjectCount, decimal TotalRaised)>> GetFounderStatsByUserIdsAsync(List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return new Dictionary<string, (int, decimal)>();

            return await _dbContext.Founders
                .AsNoTracking()
                .Where(f => userIds.Contains(f.UserId))
                .Select(f => new
                {
                    f.UserId,
                    ProjectCount = f.Projects.Count,
                    TotalRaised = f.Projects.Sum(p => p.CurrentAmount)
                })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => (x.ProjectCount, x.TotalRaised));
        }
    }
}
