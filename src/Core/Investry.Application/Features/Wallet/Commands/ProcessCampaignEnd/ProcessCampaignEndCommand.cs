using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Wallet.Commands.ProcessCampaignEnd
{
    public record ProcessCampaignEndCommand(Guid ProjectId) : IRequest<Result<bool>>;
}
