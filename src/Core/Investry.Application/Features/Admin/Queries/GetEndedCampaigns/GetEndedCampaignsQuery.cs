using Investry.Application.Common;
using Investry.Application.Common.Models;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetEndedCampaigns
{
    public record GetEndedCampaignsQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedResult<EndedCampaignDto>>>;
}
