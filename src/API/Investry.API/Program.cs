using Investry.API.Middlewares;
using Investry.Application;
using Investry.Identity;
using Investry.Infrastructure;
using Investry.Persistence;
using Investry.Persistence.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace Investry.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureInfrastructureServices(builder.Configuration);
            builder.Services.ConfigureIdentityServices(builder.Configuration);
            builder.Services.ConfigureApplicationServices();
            builder.Services.ConfigurePersistenceServices(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Investry API",
                    Version = "v1",
                    Description = "Project funding, investments and wallets.",
                    Contact = new OpenApiContact
                    {
                        Name = "Investry Team"
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT like this: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.WithOrigins(
                            "https://investry-project.vercel.app",
                            "http://localhost:5173",
                            "http://localhost:5174"
                            )
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Auto-apply pending migrations
            using (var scope = app.Services.CreateScope())
            {
                var investryDb = scope.ServiceProvider.GetRequiredService<Investry.Persistence.InvestryDbContext>();
                await investryDb.Database.MigrateAsync();

                var identityDb = scope.ServiceProvider.GetRequiredService<Investry.Identity.InvestryIdentityDbContext>();
                await identityDb.Database.MigrateAsync();
            }

            await app.Services.SeedDatabaseAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
