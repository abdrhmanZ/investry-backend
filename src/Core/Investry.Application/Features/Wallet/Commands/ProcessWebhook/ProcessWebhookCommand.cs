using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Wallet.Commands.ProcessWebhook
{
    public record ProcessWebhookCommand(string Payload, string Signature) : IRequest<Result<bool>>;
}
