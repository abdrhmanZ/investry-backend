using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Investry.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Xunit;

namespace Investry.Tests;

public class StripeWebhookTests
{
    private const string WebhookSecret = "test-webhook-secret";

    [Theory]
    [InlineData("", "signature")]
    [InlineData("{}", "")]
    public void MissingPayloadOrSignatureIsRejected(string payload, string signature)
    {
        var service = new PaymentService(Options.Create(new StripeSettings { WebhookSecret = WebhookSecret }),
            new ConfigurationBuilder().Build());

        Assert.Throws<ArgumentException>(() => service.ParseWebhookEvent(payload, signature));
    }

    [Theory]
    [InlineData("checkout.session.completed", "paid", "checkout.session.completed", 50)]
    [InlineData("checkout.session.async_payment_succeeded", "paid", "checkout.session.async_payment_succeeded", 50)]
    [InlineData("checkout.session.completed", "unpaid", "checkout.session.unpaid", 0)]
    [InlineData("charge.succeeded", "paid", "charge.succeeded", 0)]
    public void OnlyPaidCheckoutSessionsProduceADeposit(string eventType, string paymentStatus, string expectedType, decimal expectedAmount)
    {
        var payload = JsonSerializer.Serialize(new
        {
            id = "test-event",
            @object = "event",
            api_version = StripeConfiguration.ApiVersion,
            type = eventType,
            data = new
            {
                @object = new
                {
                    id = "test-session",
                    @object = eventType == "charge.succeeded" ? "charge" : "checkout.session",
                    amount_total = 5000,
                    payment_status = paymentStatus,
                    metadata = new { userId = "test-user" }
                }
            }
        });
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var digest = HMACSHA256.HashData(Encoding.UTF8.GetBytes(WebhookSecret), Encoding.UTF8.GetBytes($"{timestamp}.{payload}"));
        var signature = $"t={timestamp},v1={Convert.ToHexString(digest).ToLowerInvariant()}";
        var service = new PaymentService(Options.Create(new StripeSettings { WebhookSecret = WebhookSecret }),
            new ConfigurationBuilder().Build());

        var webhook = service.ParseWebhookEvent(payload, signature);

        Assert.Equal(expectedType, webhook.EventType);
        Assert.Equal(expectedAmount, webhook.Amount);
        if (expectedAmount > 0)
        {
            Assert.Equal("test-session", webhook.SessionId);
            Assert.Equal("test-user", webhook.Metadata["userId"]);
        }
    }
}
