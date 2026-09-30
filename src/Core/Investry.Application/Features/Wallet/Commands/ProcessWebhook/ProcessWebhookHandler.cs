using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Features.Wallet.Commands.ConfirmDeposit;
using MediatR;

namespace Investry.Application.Features.Wallet.Commands.ProcessWebhook
{
    public class ProcessWebhookHandler : IRequestHandler<ProcessWebhookCommand, Result<bool>>
    {
        private readonly IPaymentService _paymentService;
        private readonly IMediator _mediator;

        public ProcessWebhookHandler(IPaymentService paymentService, IMediator mediator)
        {
            _paymentService = paymentService;
            _mediator = mediator;
        }
        async Task<Result<bool>> IRequestHandler<ProcessWebhookCommand, Result<bool>>.Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
        {
            WebhookResult webhookResult;
            try
            {
                webhookResult = _paymentService.ParseWebhookEvent(
                    request.Payload, request.Signature);
            }
            catch (ArgumentException)
            {
                return Result<bool>.Failure(new List<Error> { new Error("Webhook.InvalidSignature", "Invalid Stripe webhook.", ErrorType.Validation) });
            }

            if (webhookResult.EventType != "checkout.session.completed" &&
                webhookResult.EventType != "checkout.session.async_payment_succeeded")
                return Result<bool>.Success(true);

            if (!webhookResult.Metadata.TryGetValue("userId", out var userId))
                return Result<bool>.Failure(new List<Error> { new Error("Webhook.InvalidMetadata", "Deposit user ID is missing.", ErrorType.Validation) });

            return await _mediator.Send(new ConfirmDepositCommand(
                SessionId: webhookResult.SessionId,
                Amount: webhookResult.Amount,
                UserId: userId
            ), cancellationToken);
        }
    }
}
