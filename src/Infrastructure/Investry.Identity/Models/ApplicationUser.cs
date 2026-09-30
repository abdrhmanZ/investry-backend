using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Investry.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public string? BanReason { get; set; }
        public DateTime? BanExpiry { get; set; }
        public KycStatus KycStatus { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
        public int EmailConfirmationRetryCount { get; set; } = 0;
        public DateTime? LastEmailSentOn { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePicturePublicId { get; set; }

        public virtual ICollection<KycVerification> KycVerifications { get; set; }

        public void SetKycStatus(KycStatus status)
        {
            KycStatus = status;
        }
    }
}
