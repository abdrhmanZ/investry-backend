using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Title)
               .IsRequired()
               .HasMaxLength(200);

            builder.Property(p => p.ShortDescription)
              .IsRequired()
              .HasMaxLength(500);

            builder.Property(p => p.TargetAmount)
               .HasColumnType("decimal(18,6)");

            builder.Property(p => p.CurrentAmount)
                   .HasColumnType("decimal(18,6)");


            builder.Property(rc => rc.MinimumContribution)
               .HasColumnType("decimal(18,6)");
            builder.HasMany(p => p.Categories)
               .WithMany(c => c.Projects);

            builder.Property(p => p.Location)
                   .HasMaxLength(200);

            builder.Property(p => p.PromotionalVideoURL)
                   .HasMaxLength(500);

            // RewardConfig (1:1)
            builder.HasOne(p => p.RewardConfig)
                   .WithOne(rc => rc.Project)
                   .HasForeignKey<RewardConfig>(rc => rc.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // EquityConfig (1:1)
            builder.HasOne(p => p.EquityConfig)
                   .WithOne(ec => ec.Project)
                   .HasForeignKey<EquityConfig>(ec => ec.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // MudarabahConfig (1:1)
            builder.HasOne(p => p.MudarabahConfig)
                   .WithOne(mc => mc.Project)
                   .HasForeignKey<MudarabahConfig>(mc => mc.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Founder Relationship
            builder.HasOne(p => p.Founder)
                .WithMany(f => f.Projects)
                .HasForeignKey(p => p.FounderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => new {p.Title,p.FounderId})
                .IsUnique() // علشان امنع تكرار المشروع للمؤسس
                .HasFilter("[IsDeleted] = 0");
            builder.HasQueryFilter(p => !p.IsDeleted); 
        }
    }
}
