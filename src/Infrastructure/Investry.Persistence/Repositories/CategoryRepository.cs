using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly InvestryDbContext _dbContext;

        public CategoryRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Category>> GetByIdsAsync(List<Guid> ids)
        {
            return await _dbContext.Categories
                                   .Where(c => ids.Contains(c.Id))
                                   .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
