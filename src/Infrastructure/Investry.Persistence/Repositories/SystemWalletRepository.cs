using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class SystemWalletRepository : Repository<SystemWallet>, ISystemWalletRepository
    {
        private readonly InvestryDbContext _dbContext;
        public SystemWalletRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SystemWallet?> GetByTypeAsync(
        SystemWalletType type)
        => await _dbContext.SystemWallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.Type == type);

        public async Task AddTransactionAsync(
            SystemWalletTransaction transaction)
            => await _dbContext.SystemWalletTransactions.AddAsync(transaction);
    }
}
