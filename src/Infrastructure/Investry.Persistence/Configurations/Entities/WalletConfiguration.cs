using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.Property(w => w.Balance)
                .HasColumnType("decimal(18,6)");

            builder.Property(w => w.OwnerType)
                .HasConversion<string>();

            builder.HasIndex(w => w.FounderId).IsUnique();
            builder.HasIndex(w => w.InvestorId).IsUnique();

            builder.HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
