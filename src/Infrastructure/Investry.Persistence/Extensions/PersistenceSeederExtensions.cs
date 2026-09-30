using Investry.Persistence.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace Investry.Persistence.Extensions
{
    public static class PersistenceSeederExtensions
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<InvestryDbContext>();

            await SystemWalletSeeder.SeedAsync(context);
        }
    }
}
