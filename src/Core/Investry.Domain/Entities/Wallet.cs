using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? InvestorId { get; set; }
        public Guid? FounderId { get; set; }

        public WalletOwnerType OwnerType { get; set; }  // Investor / Founder
        public decimal Balance { get; private set; } = 0;

        public ICollection<WalletTransaction> Transactions { get; set; }
            = new List<WalletTransaction>();

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new Exception("Amount must be positive");
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new Exception("Amount must be positive");
            if (Balance < amount) throw new Exception("Insufficient balance");
            Balance -= amount;
        }
    }
}
