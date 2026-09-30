using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        private readonly InvestryDbContext _dbContext;
        public WalletRepository(InvestryDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Wallet?> GetByInvestorUserIdAsync(string userId)
            => await _dbContext.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w =>
                    _dbContext.Investors
                        .Any(i => i.UserId == userId && i.Id == w.InvestorId));

        public async Task<Wallet?> GetByFounderUserIdAsync(string userId)
            => await _dbContext.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w =>
                    _dbContext.Founders
                        .Any(f => f.UserId == userId && f.Id == w.FounderId));

        public async Task<Wallet?> GetByUserIdAsync(string userId)
        {
            var wallet = await _dbContext.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w =>
                     _dbContext.Investors
                        .Any(i => i.UserId == userId && i.Id == w.InvestorId));

            return wallet ?? await _dbContext.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w =>
                    _dbContext.Founders
                        .Any(f => f.UserId == userId && f.Id == w.FounderId));
        }
        public async Task<WalletTransaction?> GetTransactionBySessionIdAsync(string sessionId)
            => await _dbContext.WalletTransactions.FirstOrDefaultAsync(t => t.SessionId == sessionId);
        public async Task AddTransactionAsync(WalletTransaction transaction)
        {
            await _dbContext.WalletTransactions.AddAsync(transaction);
        }

        public async Task<(IEnumerable<WalletTransaction> Items, int TotalCount)> GetTransactionsByUserIdAsync(string userId, int page, int pageSize)
        {
            var query = _dbContext.WalletTransactions
                .Where(t => _dbContext.Wallets
                        .Where(w =>
                            _dbContext.Investors.Any(i => i.UserId == userId && i.Id == w.InvestorId) ||
                            _dbContext.Founders.Any(f => f.UserId == userId && f.Id == w.FounderId))
                        .Select(w => w.Id)
                        .Contains(t.WalletId))
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
