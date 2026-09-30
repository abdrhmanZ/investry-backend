using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;

namespace Investry.Persistence.Repositories
{
    public class InvestorShareRepository : Repository<InvestorShare>, IInvestorShareRepository
    {
        private readonly InvestryDbContext _dbContext;

        public InvestorShareRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
