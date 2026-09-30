using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Investments.Commands.CreateInvestment
{
    public record CreateInvestmentCommand(
        Guid ProjectId,
        string UserId,
        decimal Amount,
        Guid? RewardTierId = null        // للـ Reward
        //decimal? EquityPercentage = null, // للـ Equity
        //decimal? ProfitSharePercentage = null// للـ Mudarabah
    ) : IRequest<Result<Guid>>;
}
