using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{

    public class ProjectMediaConfiguration : IEntityTypeConfiguration<ProjectMedia>
    {
        public void Configure(EntityTypeBuilder<ProjectMedia> builder)
        {
            builder.Property(m => m.MediaUrl)
                .IsRequired();

            builder.Property(m => m.PublicId)
                .IsRequired();

            builder.HasOne(m => m.Project)
                   .WithMany(p => p.Media)
                   .HasForeignKey(m => m.ProjectId);

            builder.HasQueryFilter(m => !m.IsDeleted);
        }
    }
}
