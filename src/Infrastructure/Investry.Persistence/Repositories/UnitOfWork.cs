using Investry.Application.Contracts.Persistence;
using Investry.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace Investry.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InvestryIdentityDbContext _identityContext;
        private readonly InvestryDbContext _investryContext;
        private IDbContextTransaction _transaction;
        private IUserRepository _userRepository;
        private IProjectRepository _projectRepository;
        private IFounderRepository _founderRepository;
        private IInvestorRepository _investorRepository;
        private IInvestmentRepository _investmentRepository;
        private IInvestorShareRepository _investorShareRepository;
        private IProfitDistributionRepository _profitDistributionRepository;
        private IInvestorProfitAllocationRepository _investorProfitAllocationRepository;
        private ICapitalReturnRepository _capitalReturnRepository;
        private ICategoryRepository _categoryRepository;
        private IProjectMediaRepository _projectMediaRepository;
        private IRewardTierRepository _rewardTierRepository;
        private IWalletRepository _walletRepository;
        private ISystemWalletRepository _systemWalletRepository;
        private ISupportTicketRepository _supportTicketRepository;

        public UnitOfWork(InvestryIdentityDbContext identitycontext, InvestryDbContext investryContext)
        {
            _identityContext = identitycontext;
            _investryContext = investryContext;
        }

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(_identityContext);
        public IProjectRepository ProjectRepository =>
            _projectRepository ??= new ProjectRepository(_investryContext);
        public IFounderRepository FounderRepository =>
            _founderRepository ??= new FounderRepository(_investryContext);
        public IInvestorRepository InvestorRepository =>
            _investorRepository ??= new InvestorRepository(_investryContext);
        public IInvestmentRepository InvestmentRepository =>
            _investmentRepository ??= new InvestmentRepository(_investryContext);
        public IInvestorShareRepository InvestorShareRepository =>
            _investorShareRepository ??= new InvestorShareRepository(_investryContext);
        public IProfitDistributionRepository ProfitDistributionRepository =>
            _profitDistributionRepository ??= new ProfitDistributionRepository(_investryContext);
        public IInvestorProfitAllocationRepository InvestorProfitAllocationRepository =>
            _investorProfitAllocationRepository ??= new InvestorProfitAllocationRepository(_investryContext);
        public ICapitalReturnRepository CapitalReturnRepository =>
            _capitalReturnRepository ??= new CapitalReturnRepository(_investryContext);
        public ICategoryRepository CategoryRepository =>
            _categoryRepository ??= new CategoryRepository(_investryContext);
        public IProjectMediaRepository ProjectMediaRepository =>
            _projectMediaRepository ??= new ProjectMediaRepository(_investryContext);
        public IRewardTierRepository RewardTierRepository =>
            _rewardTierRepository ??= new RewardTierRepository(_investryContext);
        public IWalletRepository WalletRepository =>
            _walletRepository ??= new WalletRepository(_investryContext);
        public ISystemWalletRepository SystemWalletRepository =>
            _systemWalletRepository ??= new SystemWalletRepository(_investryContext);
        public ISupportTicketRepository SupportTicketRepository =>
            _supportTicketRepository ??= new SupportTicketRepository(_investryContext);

        public void Dispose()
        {
            _identityContext.Dispose();
            _investryContext.Dispose();
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            //var username = _httpContextAccessor.HttpContext.User.FindFirst(CustomClaimTypes.Uid)?.Value;

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _identityContext.SaveChangesAsync();
            await _investryContext.SaveChangesAsync();

            scope.Complete();
        }
    }
}
