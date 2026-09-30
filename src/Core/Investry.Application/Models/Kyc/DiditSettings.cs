namespace Investry.Application.Models.Kyc
{
    public class DiditSettings
    {
        public string ApiKey { get; set; }
        public string WorkflowId { get; set; }
        public string BaseUrl { get; set; }
        public string WebhookSecret { get; set; }
        public string CallbackUrl { get; set; }
    }
}
