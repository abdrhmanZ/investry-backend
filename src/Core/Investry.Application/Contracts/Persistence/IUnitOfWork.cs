namespace Investry.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IFounderRepository FounderRepository { get; }
        IInvestorRepository InvestorRepository { get; }
        IInvestmentRepository InvestmentRepository { get; }
        IInvestorShareRepository InvestorShareRepository { get; }
        IProfitDistributionRepository ProfitDistributionRepository { get; }
        IInvestorProfitAllocationRepository InvestorProfitAllocationRepository { get; }
        ICapitalReturnRepository CapitalReturnRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProjectMediaRepository ProjectMediaRepository { get; }
        IRewardTierRepository RewardTierRepository { get; }
        IWalletRepository WalletRepository { get; }
        ISystemWalletRepository SystemWalletRepository { get; }
        ISupportTicketRepository SupportTicketRepository { get; }
        Task SaveAsync();
    }
}
