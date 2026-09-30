using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class ProfitDistributionConfiguration : IEntityTypeConfiguration<ProfitDistribution>
    {
        public void Configure(EntityTypeBuilder<ProfitDistribution> builder)
        {
            builder.Property(pd => pd.Id)
                .ValueGeneratedNever();

            builder.Property(pd => pd.PeriodStart)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(pd => pd.PeriodEnd)
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(pd => pd.NetProfit)
                .HasColumnType("decimal(18,6)")
                .IsRequired();

            builder.Property(pd => pd.InvestorsPoolProfit)
                .HasColumnType("decimal(18,6)")
                .IsRequired();

            builder.Property(pd => pd.FounderProfit)
                .HasColumnType("decimal(18,6)")
                .IsRequired();

            builder.Property(pd => pd.DistributionStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(pd => pd.DistributedAt)
                .HasColumnType("datetime2");

            builder.Property(pd => pd.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with MudarabahConfig
            builder.HasOne(pd => pd.MudarabahConfig)
                .WithMany(m => m.ProfitDistributions)
                .HasForeignKey(pd => pd.MudarabahConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with InvestorProfitAllocations
            builder.HasMany(pd => pd.InvestorProfitAllocations)
                .WithOne(ipa => ipa.ProfitDistribution)
                .HasForeignKey(ipa => ipa.ProfitDistributionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pd => pd.MudarabahConfigId);
            builder.HasIndex(pd => pd.DistributionStatus);
            builder.HasIndex(pd => pd.CreatedAt);

            builder.HasQueryFilter(pd => !pd.IsDeleted);
        }
    }
}
