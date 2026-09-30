using Investry.Application.Contracts.Persistence;
using Investry.Application.DTOs;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Investry.Identity;
using Microsoft.EntityFrameworkCore;

namespace Investry.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly InvestryIdentityDbContext _dbContext;
        public UserRepository(InvestryIdentityDbContext dbContext) => _dbContext = dbContext;
        public async Task<UserDto> GetByIdAsync(string userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                KycStatus = user.KycStatus
            };
        }

        public async Task AddKycVerificationAsync(KycVerification kyc) =>
            await _dbContext.KycVerifications.AddAsync(kyc);

        public async Task<KycVerification?> GetKycBySessionIdAsync(string sessionId)
        {
            return await _dbContext.KycVerifications
                .FirstOrDefaultAsync(k => k.ProviderSessionId == sessionId);
        }

        public async Task<KycVerification?> GetActiveKycForUserAsync(string userId)
        {
            return await _dbContext.KycVerifications
                .Where(k => k.UserId == userId && k.Status == KycStatus.Pending)
                .OrderByDescending(k => k.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateKycStatusAsync(string userId, KycStatus status)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.SetKycStatus(status);
            }
        }
    }
}
