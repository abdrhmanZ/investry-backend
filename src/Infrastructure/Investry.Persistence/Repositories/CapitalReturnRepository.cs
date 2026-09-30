using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;

namespace Investry.Persistence.Repositories
{
    public class CapitalReturnRepository : Repository<CapitalReturn>, ICapitalReturnRepository
    {
        private readonly InvestryDbContext _dbContext;

        public CapitalReturnRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
