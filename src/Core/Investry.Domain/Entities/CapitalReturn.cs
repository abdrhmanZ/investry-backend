using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    // دي هنسجل فيها رأس المال اللي هترجع في نهاية المضاربة
    public class CapitalReturn
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid InvestorId { get; set; }
        public Investor Investor { get; set; }

        public Guid InvestmentId { get; set; }
        public Investment Investment { get; set; }

        public Guid MudarabahConfigId { get; set; }
        public MudarabahConfig MudarabahConfig { get; set; }

        public decimal OriginalAmount { get; set; }
        public decimal ReturnedAmount { get; set; }

        public CapitalReturnStatus CapitalReturnStatus { get; set; }

        public DateTime? ReturnedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
