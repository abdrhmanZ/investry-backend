using Investry.Application.Common;
using Investry.Application.Common.Models;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetEndedCampaigns
{
    public class GetEndedCampaignsHandler : IRequestHandler<GetEndedCampaignsQuery, Result<PagedResult<EndedCampaignDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public GetEndedCampaignsHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }
        public async Task<Result<PagedResult<EndedCampaignDto>>> Handle(GetEndedCampaignsQuery request, CancellationToken cancellationToken)
        {
            var (projects, totalCount) = await _unitOfWork.ProjectRepository
            .GetEndedCampaignsForAdminAsync(request.Page, request.PageSize);

            var userIds = projects
            .Select(p => p.Founder.UserId)
            .Distinct()
            .ToList();

            var userInfos = await _identityService
                .GetUserInfoByIdsAsync(userIds);

            var dtos = projects.Select(p =>
            {
                // جيب الـ info من الـ Dictionary
                userInfos.TryGetValue(p.Founder.UserId, out var founderInfo);

                return new EndedCampaignDto(
                    ProjectId: p.Id,
                    ProjectTitle: p.Title,
                    FounderName: founderInfo.Name ?? "Unknown",
                    FounderEmail: founderInfo.Email ?? "Unknown",
                    TargetAmount: p.TargetAmount,
                    CollectedAmount: p.CurrentAmount,
                    FundingProgressPercentage: p.TargetAmount > 0 ? (int)((p.CurrentAmount / p.TargetAmount) * 100) : 0,
                    InvestorsCount: p.Investments.Count,
                    EndDate: p.EndDate,
                    EscrowAmount: p.CurrentAmount,
                    ReleaseStatus: p.ProjectStatus switch
                    {
                        ProjectStatus.Successful => CampaignReleaseStatus.Released.ToString(),
                        ProjectStatus.Failed => CampaignReleaseStatus.Refunded.ToString(),
                        _ => CampaignReleaseStatus.PendingRelease.ToString()
                    }
                );
            });

            return Result<PagedResult<EndedCampaignDto>>.Success(
            new PagedResult<EndedCampaignDto>(
                Items: dtos,
                TotalCount: totalCount,
                Page: request.Page,
                PageSize: request.PageSize
            ));
        }
    }
}
