using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class CapitalReturnConfiguration : IEntityTypeConfiguration<CapitalReturn>
    {
        public void Configure(EntityTypeBuilder<CapitalReturn> builder)
        {
            builder.Property(cr => cr.Id)
               .ValueGeneratedNever();

            builder.Property(cr => cr.OriginalAmount)
                .HasColumnType("decimal(18,6)")
                .IsRequired();

            builder.Property(cr => cr.ReturnedAmount)
                .HasColumnType("decimal(18,6)")
                .IsRequired();

            builder.Property(cr => cr.CapitalReturnStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(cr => cr.ReturnedAt)
                .HasColumnType("datetime2");

            builder.Property(cr => cr.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with Investor
            builder.HasOne(cr => cr.Investor)
                .WithMany(i => i.CapitalReturns)
                .HasForeignKey(cr => cr.InvestorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relationship with Investment
            builder.HasOne(cr => cr.Investment)
                .WithMany()
                .HasForeignKey(cr => cr.InvestmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relationship with Project
            builder.HasOne(cr => cr.MudarabahConfig)
                .WithMany(mc => mc.CapitalReturns)
                .HasForeignKey(cr => cr.MudarabahConfigId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            builder.HasIndex(cr => cr.InvestorId);
            builder.HasIndex(cr => cr.InvestmentId);
            builder.HasIndex(cr => cr.MudarabahConfigId);
            builder.HasIndex(cr => cr.CapitalReturnStatus);

            builder.HasQueryFilter(cr => !cr.IsDeleted);
        }
    }
}
