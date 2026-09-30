using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class MudarabahConfigConfiguration : IEntityTypeConfiguration<MudarabahConfig>
    {
        public void Configure(EntityTypeBuilder<MudarabahConfig> builder)
        {
            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(mc => mc.InvestorsProfitSharePercentage)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(mc=>mc.DurationInMonths)
                .IsRequired();

            builder.Property(mc=>mc.ProfitDistributionFrequency)
                .IsRequired();

            builder.Property(m => m.MudarabahStartDate)
                .HasColumnType("datetime2");

            builder.Property(m => m.MudarabahEndDate)
                .HasColumnType("datetime2");

            // Relationship with ProfitDistribution
            builder.HasMany(m => m.ProfitDistributions)
                .WithOne(pd => pd.MudarabahConfig)
                .HasForeignKey(pd => pd.MudarabahConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(mc => !mc.IsDeleted);
        }
    }
}
