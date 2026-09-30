using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    public class SystemWallet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public SystemWalletType Type { get; set; }  // Escrow / Platform
        public decimal Balance { get; private set; } = 0;

        public ICollection<SystemWalletTransaction> Transactions { get; set; }
            = new List<SystemWalletTransaction>();

        public void Credit(decimal amount)
        {
            if (amount <= 0) throw new Exception("Amount must be positive");
            Balance += amount;
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0) throw new Exception("Amount must be positive");
            if (Balance < amount) throw new Exception("Insufficient balance");
            Balance -= amount;
        }
    }
}
