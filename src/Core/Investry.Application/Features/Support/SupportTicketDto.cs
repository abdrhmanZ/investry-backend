namespace Investry.Application.Features.Support
{
    public class SupportTicketDto
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? AdminReply { get; set; }
        public DateTime? RepliedAt { get; set; }
    }
}
