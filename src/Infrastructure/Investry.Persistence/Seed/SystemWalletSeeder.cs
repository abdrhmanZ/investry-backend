using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Seed
{
    public static class SystemWalletSeeder
    {
        public static async Task SeedAsync(InvestryDbContext context)
        {
            if (!await context.SystemWallets.AnyAsync())
            {
                context.SystemWallets.AddRange(
                    new SystemWallet { Type = SystemWalletType.Escrow },
                    new SystemWallet { Type = SystemWalletType.Platform }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
