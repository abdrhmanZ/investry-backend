using Investry.Domain.Entities;
using Investry.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Investry.Identity
{
    public class InvestryIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<KycVerification> KycVerifications { get; set; }
        public InvestryIdentityDbContext(DbContextOptions<InvestryIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(InvestryIdentityDbContext).Assembly);

            builder.Entity<ApplicationUser>()
                .OwnsMany(u => u.RefreshTokens);

            builder.Entity<KycVerification>(entity =>
            {
                entity.HasKey(k => k.Id);

                entity.HasOne<ApplicationUser>()
                      .WithMany(u => u.KycVerifications)
                      .HasForeignKey(k => k.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
