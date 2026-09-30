using Investry.Application.Common;
using Investry.Application.Models.Identity;

namespace Investry.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(AuthRequest request);
        Task<Result<string>> RegisterAsync(RegistrationRequest request, string role);
        Task<Result<string>> ConfirmEmailAsync(string userId, string token);
        Task<Result<string>> ResendConfirmationEmailAsync(string email);
        Task<Result<AuthResponse>> RefreshTokenAsync(string token);
        Task<Result<bool>> RevokeTokenAsync(string token);
        Task<Result<string>> ForgotPasswordAsync(string email);
        Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<Result<string>> LogoutAsync(string refreshToken);
        Task<Result<AuthResponse>> SignInWithGoogleAsync(string idToken, string role);
    }
}
