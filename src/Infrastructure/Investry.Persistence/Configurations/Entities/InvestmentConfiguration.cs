using Investry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investry.Persistence.Configurations.Entities
{
    public class InvestmentConfiguration : IEntityTypeConfiguration<Investment>
    {
        public void Configure(EntityTypeBuilder<Investment> builder)
        {
            builder.Property(i => i.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,6)");

            // investor relationship
            builder.HasOne(x => x.Investor)
                .WithMany(i => i.Investments)
                .HasForeignKey(x => x.InvestorId)
                .OnDelete(DeleteBehavior.Cascade);

            // project relationship
            builder.HasOne(x => x.Project)
                .WithMany(p => p.Investments)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // reward tier relationship (opptional) علشان ممكن المستثمر مايختارش reward model
            builder.HasOne(i => i.RewardTier)
               .WithMany(rt => rt.Investments)
               .HasForeignKey(i => i.RewardTierId)
               .OnDelete(DeleteBehavior.Restrict);

            // علشان امنع الاستثمار المتكرر في نفس ال tier 
            builder.HasIndex(i => new { i.InvestorId, i.RewardTierId })
               .IsUnique()
               .HasFilter("[RewardTierId] IS NOT NULL");
        }
    }
}
