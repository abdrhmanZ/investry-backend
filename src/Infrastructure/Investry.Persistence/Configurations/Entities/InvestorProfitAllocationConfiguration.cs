using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class InvestorProfitAllocationConfiguration : IEntityTypeConfiguration<InvestorProfitAllocation>
    {
        public void Configure(EntityTypeBuilder<InvestorProfitAllocation> builder)
        {
            builder.Property(ipa => ipa.Id)
                .ValueGeneratedNever();

            builder.Property(ipa => ipa.CapitalRatio)
                .HasColumnType("decimal(18,10)")
                .IsRequired();

            builder.Property(ipa => ipa.AllocatedProfit)
               .HasColumnType("decimal(18,6)")
               .IsRequired();

            builder.Property(ipa => ipa.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with ProfitDistribution
            builder.HasOne(ipa => ipa.ProfitDistribution)
                .WithMany(pd => pd.InvestorProfitAllocations)
                .HasForeignKey(ipa => ipa.ProfitDistributionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Investor
            builder.HasOne(ipa => ipa.Investor)
                .WithMany(i => i.ProfitAllocations)
                .HasForeignKey(ipa => ipa.InvestorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relationship with Investment
            builder.HasOne(ipa => ipa.Investment)
                .WithMany()
                .HasForeignKey(ipa => ipa.InvestmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            builder.HasIndex(ipa => ipa.ProfitDistributionId);
            builder.HasIndex(ipa => ipa.InvestorId);
            builder.HasIndex(ipa => ipa.InvestmentId);

            builder.HasQueryFilter(ipa => !ipa.IsDeleted);
        }
    }
}
