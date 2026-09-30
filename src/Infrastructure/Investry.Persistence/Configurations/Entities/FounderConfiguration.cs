using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class FounderConfiguration : IEntityTypeConfiguration<Founder>
    {
        public void Configure(EntityTypeBuilder<Founder> builder)
        {
            builder.HasOne(f => f.Wallet)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.FounderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
