using Investry.Application.Common;
using Investry.Application.DTOs;
using MediatR;

namespace Investry.Application.Features.KycVerifications.Commands.HandleKycWebhook
{
    public class HandleKycWebhookCommand : IRequest<Result<Unit>>
    {
        public DiditWebhookDto Webhook { get; }
        public string Signature { get; }
        public string RawJson { get; }

        public HandleKycWebhookCommand(DiditWebhookDto webhook, string signature, string rawJson)
        {
            Webhook = webhook;
            Signature = signature;
            RawJson = rawJson;
        }

        public string SessionId => Webhook.SessionId;
        public string Status => Webhook.Status;
    }
}
