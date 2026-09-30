namespace Investry.Application.Contracts.Infrastructure
{
    public interface IUrlGenerator
    {
        string GenerateEmailConfirmationLink(string userId, string token);
        string GeneratePasswordResetLink(string userId, string token);
    }
}
