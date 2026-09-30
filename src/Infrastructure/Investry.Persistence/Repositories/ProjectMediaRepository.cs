using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;

namespace Investry.Persistence.Repositories
{
    public class ProjectMediaRepository : Repository<ProjectMedia>, IProjectMediaRepository
    {
        private readonly InvestryDbContext _dbContext;
        public ProjectMediaRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
