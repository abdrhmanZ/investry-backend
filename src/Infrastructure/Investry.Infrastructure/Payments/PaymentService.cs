using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Investry.Infrastructure.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;

        private readonly IConfiguration _configuration;
        public PaymentService(IOptions<StripeSettings> stripeSettings, IConfiguration configuration)
        {
            _stripeSettings = stripeSettings.Value;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }
        public async Task<CheckoutResponse> CreateCheckoutSessionAsync(decimal amount, string userId, Dictionary<string, string> metadata)
        {
            try
            {
                var baseUrl = _configuration["Frontend:BaseUrl"];
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                UnitAmount = (long)(amount * 100), // هحولخا لاصغر وحده
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Wallet Deposit",
                                    Description = $"Deposit funds into your Investry account wallet.",
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = $"{baseUrl}/wallet/success",
                    CancelUrl = $"{baseUrl}/wallet/cancel",
                    Metadata = metadata
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return new CheckoutResponse
                {
                    SessionId = session.Id,
                    CheckoutUrl = session.Url
                };
            }
            catch (StripeException e)
            {
                throw new Exception($"Stripe error: {e.Message}");
            }
        }

        public WebhookResult ParseWebhookEvent(string payload, string signature)
        {
            if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(signature))
                throw new ArgumentException("Webhook payload and signature are required.");

            if (string.IsNullOrWhiteSpace(_stripeSettings.WebhookSecret))
                throw new InvalidOperationException("Stripe webhook secret is not configured.");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(payload, signature, _stripeSettings.WebhookSecret);

                if (stripeEvent.Type != "checkout.session.completed" &&
                    stripeEvent.Type != "checkout.session.async_payment_succeeded")
                    return new WebhookResult(stripeEvent.Type, "", 0, new());

                var session = (Session)stripeEvent.Data.Object;
                if (session.PaymentStatus != "paid")
                    return new WebhookResult("checkout.session.unpaid", session.Id, 0, session.Metadata);

                return new WebhookResult(
                    EventType: stripeEvent.Type,
                    SessionId: session.Id,
                    Amount: session.AmountTotal!.Value / 100m,
                    Metadata: session.Metadata
                    );
            }
            catch (StripeException e)
            {
                throw new ArgumentException("Invalid Stripe webhook.", nameof(payload), e);
            }
        }
    }
}
