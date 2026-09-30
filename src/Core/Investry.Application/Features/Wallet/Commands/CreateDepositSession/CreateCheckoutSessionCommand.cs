using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using MediatR;

namespace Investry.Application.Features.Wallet.Commands.CreateDepositSession
{
    public record CreateCheckoutSessionCommand(string UserId, decimal Amount) : IRequest<Result<CheckoutResponse>>;
}
