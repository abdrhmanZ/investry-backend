using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Persistence.Configurations.Entities
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.HasData(
                new Category
                {
                    Id = Guid.Parse("f9f0a28a-723a-4a6c-9a4a-2e38c3e3e3e3"),
                    Name = "Technology",
                    Description = "Projects related to software, hardware, and internet services."
                },
                new Category
                {
                    Id = Guid.Parse("c1d5e8f0-1a2b-3c4d-5e6f-7a8b9c0d1e2f"),
                    Name = "Healthcare",
                    Description = "Projects in the medical, wellness, and health tech fields."
                },
                new Category
                {
                    Id = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                    Name = "Art & Culture",
                    Description = "Creative projects including film, music, design, and visual arts."
                },
                new Category
                {
                    Id = Guid.Parse("b1c2d3e4-f5a6-b7c8-d9e0-f1a2b3c4d5e6"),
                    Name = "Food & Beverage",
                    Description = "Restaurants, food products, and innovative culinary ventures."
                },
                new Category
                {
                    Id = Guid.Parse("d1e2f3a4-b5c6-d7e8-f9a0-1b2c3d4e5f6a"),
                    Name = "Education",
                    Description = "Educational tools, platforms, and learning resources."
                },
                new Category
                {
                    Id = Guid.Parse("e1f2a3b4-c5d6-e7f8-a9b0-1c2d3e4f5a6b"),
                    Name = "Environment",
                    Description = "Projects focused on sustainability, conservation, and green technology."
                },
                new Category
                {
                    Id = Guid.Parse("f1a2b3c4-d5e6-f7a8-9b0c-1d2e3f4a5b6c"),
                    Name = "Fashion",
                    Description = "Clothing, accessories, and innovative fashion-related projects."
                }

            );
        }
    }
}
