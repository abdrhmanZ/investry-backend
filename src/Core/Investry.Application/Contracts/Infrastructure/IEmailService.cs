using Investry.Application.Models.Email;

namespace Investry.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendAsync(Email email);
    }
}
