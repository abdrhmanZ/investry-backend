using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class RewardConfigConfiguration : IEntityTypeConfiguration<RewardConfig>
    {
        public void Configure(EntityTypeBuilder<RewardConfig> builder)
        {
            builder.HasOne(rc => rc.Project)
               .WithOne(p => p.RewardConfig)
               .HasForeignKey<RewardConfig>(rc => rc.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(rc => !rc.IsDeleted);
        }
    }
}
