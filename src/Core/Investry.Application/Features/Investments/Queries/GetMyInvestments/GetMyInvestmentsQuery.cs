using Investry.Application.Common;
using MediatR;

namespace Investry.Application.Features.Investments.Queries.GetMyInvestments
{
    public record GetMyInvestmentsQuery(string UserId)
        : IRequest<Result<IReadOnlyList<InvestorInvestmentDto>>>;
}
