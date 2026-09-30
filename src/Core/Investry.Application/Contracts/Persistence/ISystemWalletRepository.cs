using Investry.Domain.Entities;
using Investry.Domain.Enums;

namespace Investry.Application.Contracts.Persistence
{
    public interface ISystemWalletRepository : IRepository<SystemWallet>
    {
        Task<SystemWallet?> GetByTypeAsync(SystemWalletType type);
        Task AddTransactionAsync(SystemWalletTransaction transaction);
    }
}
