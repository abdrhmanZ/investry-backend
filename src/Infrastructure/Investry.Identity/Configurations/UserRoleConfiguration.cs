using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investry.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
                    UserId = "9e224968-33e4-4652-b7b7-8574d048cdb9"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "cbc43a8e-f7bb-4445-baaf-1add431ffbbf",
                    UserId = "e4a1c9b2-6f3d-4a8e-9c71-1b5f8d2a0e34"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "9f2b7d4a-3c81-4e6a-b1d9-7a4f5c2e8d10",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                }
            );
        }
    }
}
