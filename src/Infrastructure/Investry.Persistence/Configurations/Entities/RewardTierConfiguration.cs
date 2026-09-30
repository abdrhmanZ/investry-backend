using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class RewardTierConfiguration : IEntityTypeConfiguration<RewardTier>
    {
        public void Configure(EntityTypeBuilder<RewardTier> builder)
        {
            builder.Property(rt => rt.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(rt => rt.Description)
               .HasMaxLength(500);

            builder.Property(rt => rt.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,6)");

            builder.HasOne(rt => rt.RewardConfig)
                .WithMany(rc => rc.RewardTiers)
                .HasForeignKey(rt => rt.RewardConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            // مش متكرر لكل مشروع tier علشان يبقا اسم ال 
            builder.HasIndex(rt => new { rt.RewardConfigId, rt.Title })
                .IsUnique();

            builder.HasQueryFilter(rt => !rt.IsDeleted);
        }
    }
}
