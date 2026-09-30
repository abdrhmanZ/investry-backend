using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class SystemWalletTransactionConfiguration : IEntityTypeConfiguration<SystemWalletTransaction>
    {
        public void Configure(EntityTypeBuilder<SystemWalletTransaction> builder)
        {
            builder.Property(x => x.Amount)
               .HasPrecision(18, 6);

            builder.HasOne(x => x.SystemWallet)
                   .WithMany(w => w.Transactions)
                   .HasForeignKey(x => x.SystemWalletId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Project)
                   .WithMany()
                   .HasForeignKey(x => x.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Investment)
                   .WithMany()
                   .HasForeignKey(x => x.InvestmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
