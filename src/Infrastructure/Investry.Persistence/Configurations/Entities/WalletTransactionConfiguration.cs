using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,6)");

            builder.Property(t => t.Type)
                .HasConversion<string>();

            builder.Property(t => t.Status)
                .HasConversion<string>();

            builder.Property(t => t.SessionId)
                .HasMaxLength(255);

            builder.HasIndex(t => t.SessionId);
        }
    }
}
