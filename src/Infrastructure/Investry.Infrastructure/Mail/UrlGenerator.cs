using Investry.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Investry.Infrastructure.Mail
{
    public class UrlGenerator : IUrlGenerator
    {
        private readonly IConfiguration _configuration;
        public UrlGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateEmailConfirmationLink(string userId, string token)
        {
            //var baseUrl = _configuration["AppSettings:BaseUrl"];
            //var baseUrl = _configuration["ngrok:BaseUrl"];
            //var baseUrl = _configuration["Monster:BaseUrl"];
            var baseUrl = _configuration["Frontend:BaseUrl"];

            return $"{baseUrl}/email-confirm?userId={userId}&token={Uri.EscapeDataString(token)}";
        }

        public string GeneratePasswordResetLink(string userId, string token)
        {
            //var baseUrl = _configuration["AppSettings:BaseUrl"];
            //var baseUrl = _configuration["ngrok:BaseUrl"];
            //var baseUrl = _configuration["Monster:BaseUrl"];
            var baseUrl = _configuration["Frontend:BaseUrl"];

            return $"{baseUrl}/reset-password?userId={userId}&token={Uri.EscapeDataString(token)}";
        }
    }
}
