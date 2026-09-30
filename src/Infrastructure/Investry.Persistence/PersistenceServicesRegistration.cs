using Investry.Application.Contracts.Persistence;
using Investry.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Investry.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InvestryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("InvestryConnectionString"),
                b => b.MigrationsAssembly(typeof(InvestryDbContext).Assembly.FullName)));
            //services.AddScoped<typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IFounderRepository, FounderRepository>();
            services.AddScoped<IInvestorRepository, InvestorRepository>();
            services.AddScoped<IInvestmentRepository, InvestmentRepository>();

            return services;
        }
    }
}
