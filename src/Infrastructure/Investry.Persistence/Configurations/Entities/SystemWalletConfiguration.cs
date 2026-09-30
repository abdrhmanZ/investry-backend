using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class SystemWalletConfiguration : IEntityTypeConfiguration<SystemWallet>
    {
        public void Configure(EntityTypeBuilder<SystemWallet> builder)
        {
            builder.Property(w => w.Balance)
            .HasColumnType("decimal(18,6)");

            builder.Property(w => w.Type)
                .HasConversion<string>();

            builder.HasIndex(w => w.Type).IsUnique();

            builder.HasMany(w => w.Transactions)
                .WithOne(t => t.SystemWallet)
                .HasForeignKey(t => t.SystemWalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
