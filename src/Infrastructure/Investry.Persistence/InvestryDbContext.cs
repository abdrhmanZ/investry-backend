using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence
{
    public class InvestryDbContext : DbContext
    {
        public InvestryDbContext(DbContextOptions<InvestryDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvestryDbContext).Assembly);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<RewardConfig> RewardConfigs { get; set; }
        public DbSet<RewardTier> RewardTiers { get; set; }
        public DbSet<Founder> Founders { get; set; }
        public DbSet<Investor> Investors { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<EquityConfig> EquityConfigs { get; set; }
        public DbSet<InvestorShare> InvestorShares { get; set; }
        public DbSet<MudarabahConfig> MudarabahConfigs { get; set; }
        public DbSet<ProfitDistribution> ProfitDistributions { get; set; }
        public DbSet<InvestorProfitAllocation> InvestorProfitAllocations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<SystemWallet> SystemWallets { get; set; }
        public DbSet<SystemWalletTransaction> SystemWalletTransactions { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
    }
}
