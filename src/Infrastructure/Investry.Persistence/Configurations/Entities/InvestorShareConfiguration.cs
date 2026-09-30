using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class InvestorShareConfiguration : IEntityTypeConfiguration<InvestorShare>
    {
        public void Configure(EntityTypeBuilder<InvestorShare> builder)
        {
            builder.Property(s=>s.AmountInvested)
                .IsRequired()
                .HasColumnType("decimal(18,6)");

            builder.Property(s => s.SharesPercentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

             builder.HasOne(s => s.Investor)
                .WithMany(i => i.InvestorShares)
                .HasForeignKey(s => s.InvestorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Investment)
                .WithOne()
                .HasForeignKey<InvestorShare>(s => s.InvestmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
