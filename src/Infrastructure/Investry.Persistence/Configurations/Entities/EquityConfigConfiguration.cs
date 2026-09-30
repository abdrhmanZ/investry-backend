using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class EquityConfigConfiguration : IEntityTypeConfiguration<EquityConfig>
    {
        public void Configure(EntityTypeBuilder<EquityConfig> builder)
        {
            builder.Property(ec => ec.EquityPercentage)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.HasMany(ec => ec.InvestorShares)
                .WithOne(s => s.EquityConfig)
                .HasForeignKey(s => s.EquityConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(ec => !ec.IsDeleted);
        }
    }
}
