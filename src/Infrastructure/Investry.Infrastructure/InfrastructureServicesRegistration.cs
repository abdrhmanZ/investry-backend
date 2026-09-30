using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Models.Media;
using Investry.Application.Models.Email;
using Investry.Application.Models.Kyc;
using Investry.Infrastructure.Kyc.Providers;
using Investry.Infrastructure.Mail;
using Investry.Infrastructure.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Investry.Infrastructure.Payments;

namespace Investry.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IUrlGenerator, UrlGenerator>();

            services.Configure<DiditSettings>(configuration.GetSection("Didit"));

            services.AddHttpClient<DiditKycProvider>(client =>
            {
                client.BaseAddress = new Uri(configuration["Didit:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Didit:ApiKey"]}");
            });

            services.AddScoped<IKycProvider, DiditKycProvider>();

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IMediaService, MediaService>();

            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
