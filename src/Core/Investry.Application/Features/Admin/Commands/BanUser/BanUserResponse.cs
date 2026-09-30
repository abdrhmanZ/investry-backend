namespace Investry.Application.Features.Admin.Commands.BanUser
{
    public class BanUserResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string BanReason { get; set; } = string.Empty;
        public DateTime? BanExpiry { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
