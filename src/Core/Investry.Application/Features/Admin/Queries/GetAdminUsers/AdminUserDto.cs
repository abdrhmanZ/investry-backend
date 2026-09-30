namespace Investry.Application.Features.Admin.Queries.GetAdminUsers
{
    public class AdminUserDto
    {
        // Identity Info
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string KycStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Founder-specific stats (null for Investors)
        public int? ProjectCount { get; set; }
        public decimal? TotalRaised { get; set; }

        // Investor-specific stats (null for Founders)
        public int? InvestmentCount { get; set; }
        public decimal? TotalInvested { get; set; }

        // Ban info (null if not banned)
        public string? BanReason { get; set; }
        public DateTime? BanExpiry { get; set; }
    }
}
