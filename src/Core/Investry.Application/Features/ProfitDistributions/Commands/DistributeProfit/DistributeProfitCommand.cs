using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.ProfitDistributions.Commands.DistributeProfit
{
    public record DistributeProfitCommand(
        DistributeProfitRequest DistributeProfitRequest,
        string UserId
        ) : IRequest<Result<Guid>>;
}
