using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    public class KycVerification
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ProviderSessionId { get; set; }
        public string VerificationUrl { get; set; }
        public KycStatus Status { get; set; } = KycStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedAt { get; set; }
    }
}
