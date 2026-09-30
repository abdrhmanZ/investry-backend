using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    public class SystemWalletTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SystemWalletId { get; set; }
        public Guid? ProjectId { get; set; }        // مرتبطة بأنهي project
        public Guid? InvestmentId { get; set; }     // مرتبطة بأنهي investment
        public decimal Amount { get; set; }
        public SystemTransactionType Type { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public SystemWallet SystemWallet { get; set; }
        public Project? Project { get; set; }
        public Investment? Investment { get; set; }
    }
}
