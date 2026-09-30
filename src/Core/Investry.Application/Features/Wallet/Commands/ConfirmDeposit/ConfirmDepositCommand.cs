using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Wallet.Commands.ConfirmDeposit
{
    public record ConfirmDepositCommand(string SessionId, decimal Amount, string UserId) : IRequest<Result<bool>>;
}
