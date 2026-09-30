using Investry.Application.DTOs;
using Investry.Domain.Entities;
using Investry.Domain.Enums;

namespace Investry.Application.Contracts.Persistence
{
    public interface IUserRepository
    {
        Task<UserDto> GetByIdAsync(string userId);
        Task AddKycVerificationAsync(KycVerification kyc);
        Task<KycVerification?> GetKycBySessionIdAsync(string sessionId);
        Task<KycVerification?> GetActiveKycForUserAsync(string userId);
        Task UpdateKycStatusAsync(string userId, KycStatus status);
    }
}
