using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class InvestorConfiguration : IEntityTypeConfiguration<Investor>
    {
        public void Configure(EntityTypeBuilder<Investor> builder)
        {
            builder.HasMany(i => i.Investments)
               .WithOne(inv => inv.Investor)
               .HasForeignKey(inv => inv.InvestorId)
               .OnDelete(DeleteBehavior.Cascade);

            // Relationship with ProfitAllocations
            builder.HasMany(i => i.ProfitAllocations)
                .WithOne(pa => pa.Investor)
                .HasForeignKey(pa => pa.InvestorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relationship with CapitalReturns
            builder.HasMany(i => i.CapitalReturns)
                .WithOne(cr => cr.Investor)
                .HasForeignKey(cr => cr.InvestorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(i => i.Wallet)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.InvestorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
