using Investry.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investry.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasData(
                 new ApplicationUser
                 {
                     Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                     Email = "admin@localhost.com",
                     NormalizedEmail = "ADMIN@LOCALHOST.COM",
                     FirstName = "System",
                     LastName = "Admin",
                     UserName = "admin@localhost.com",
                     NormalizedUserName = "ADMIN@LOCALHOST.COM",
                     PasswordHash = null,
                     EmailConfirmed = true
                 },
                 new ApplicationUser
                 {
                     Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                     Email = "investor@localhost.com",
                     NormalizedEmail = "INVESTOR@LOCALHOST.COM",
                     FirstName = "System",
                     LastName = "Investor",
                     UserName = "investor@localhost.com",
                     NormalizedUserName = "INVESTOR@LOCALHOST.COM",
                     PasswordHash = null,
                     EmailConfirmed = true
                 },
                 new ApplicationUser
                 {
                     Id = "e4a1c9b2-6f3d-4a8e-9c71-1b5f8d2a0e34",
                     Email = "founder@localhost.com",
                     NormalizedEmail = "FOUNDER@LOCALHOST.COM",
                     FirstName = "System",
                     LastName = "Founder",
                     UserName = "founder@localhost.com",
                     NormalizedUserName = "FOUNDER@LOCALHOST.COM",
                     PasswordHash = null,
                     EmailConfirmed = true
                 }
            );
        }
    }
}
