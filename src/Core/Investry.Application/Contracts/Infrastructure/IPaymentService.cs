namespace Investry.Application.Contracts.Infrastructure
{
    public class CheckoutResponse
    {
        public string SessionId { get; set; }
        public string CheckoutUrl { get; set; }
    }

    public record WebhookResult(
        string EventType,
        string SessionId,
        decimal Amount,
        Dictionary<string, string> Metadata
    );

    public interface IPaymentService
    {
        Task<CheckoutResponse> CreateCheckoutSessionAsync(decimal amount, string userId, Dictionary<string, string> metadata);
        WebhookResult ParseWebhookEvent(string payload, string signature);
    }
}
