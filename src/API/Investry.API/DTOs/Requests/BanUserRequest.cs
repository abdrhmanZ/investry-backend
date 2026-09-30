namespace Investry.API.DTOs.Requests
{
    public class BanUserRequest
    {
        public int DurationInDays { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
