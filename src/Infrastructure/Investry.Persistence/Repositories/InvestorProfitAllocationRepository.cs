using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investry.Persistence.Repositories
{
    public class InvestorProfitAllocationRepository : Repository<InvestorProfitAllocation>, IInvestorProfitAllocationRepository
    {
        private readonly InvestryDbContext _dbContext;
        public InvestorProfitAllocationRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
