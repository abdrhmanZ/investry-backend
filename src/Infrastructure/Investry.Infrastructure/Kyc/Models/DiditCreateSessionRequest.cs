namespace Investry.Infrastructure.Kyc.Models
{
    public class DiditCreateSessionRequest
    {
        public string workflow_id { get; set; }
        public string vendor_data { get; set; }
        public string callback { get; set; }
        public string callback_method { get; set; } = "both";
        public string language { get; set; } = "en";
    }
}
