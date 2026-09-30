namespace Investry.API.DTOs.Requests
{
    public class CreateTicketRequest
    {
        public string Category { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
