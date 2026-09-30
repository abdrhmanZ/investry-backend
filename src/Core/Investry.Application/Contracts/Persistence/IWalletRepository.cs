using Investry.Domain.Entities;

namespace Investry.Application.Contracts.Persistence
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<Wallet?> GetByUserIdAsync(string userId);
        Task<Wallet?> GetByInvestorUserIdAsync(string userId);
        Task<Wallet?> GetByFounderUserIdAsync(string userId);
        Task<WalletTransaction?> GetTransactionBySessionIdAsync(string sessionId);
        Task AddTransactionAsync(WalletTransaction transaction);
        Task<(IEnumerable<WalletTransaction> Items, int TotalCount)> GetTransactionsByUserIdAsync(string userId, int page, int pageSize);
    }
}
