using Investry.Application.Common;
using Investry.Application.Common.Models;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminUsers
{
    public class GetAdminUsersHandler
        : IRequestHandler<GetAdminUsersQuery, Result<PagedResult<AdminUserDto>>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminUsersHandler(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<AdminUserDto>>> Handle(
            GetAdminUsersQuery request, CancellationToken cancellationToken)
        {
            var pageSize = Math.Clamp(request.PageSize, 1, 50);
            var page = Math.Max(request.Page, 1);

            var role = ParseRole(request.Role);
            var status = ParseStatus(request.Status);

            var (users, totalCount) = await _identityService
                .GetAdminUsersAsync(page, pageSize, role, status, request.Search);

            if (users.Count > 0)
                await PopulateRoleStats(users);

            return Result<PagedResult<AdminUserDto>>.Success(
                new PagedResult<AdminUserDto>(users, totalCount, page, pageSize));
        }

        private static UserRole? ParseRole(string? role)
        {
            if (string.IsNullOrWhiteSpace(role) || role.Equals("All", StringComparison.OrdinalIgnoreCase))
                return null;

            return Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed)
                ? parsed
                : null;
        }

        private static AccountStatus? ParseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status) || status.Equals("All", StringComparison.OrdinalIgnoreCase))
                return null;

            if (status.Equals("Banned", StringComparison.OrdinalIgnoreCase))
                return AccountStatus.Suspended;

            if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return AccountStatus.Active;

            return null;
        }

        private async Task PopulateRoleStats(List<AdminUserDto> users)
        {
            var founderUserIds = users
                .Where(u => u.Role == nameof(UserRole.Founder))
                .Select(u => u.UserId).ToList();

            var investorUserIds = users
                .Where(u => u.Role == nameof(UserRole.Investor))
                .Select(u => u.UserId).ToList();

            if (founderUserIds.Count > 0)
            {
                var founderStats = await _unitOfWork.FounderRepository
                    .GetFounderStatsByUserIdsAsync(founderUserIds);

                foreach (var user in users.Where(u => u.Role == nameof(UserRole.Founder)))
                {
                    if (founderStats.TryGetValue(user.UserId, out var stats))
                    {
                        user.ProjectCount = stats.ProjectCount;
                        user.TotalRaised = stats.TotalRaised;
                    }
                    else
                    {
                        user.ProjectCount = 0;
                        user.TotalRaised = 0;
                    }
                }
            }

            if (investorUserIds.Count > 0)
            {
                var investorStats = await _unitOfWork.InvestorRepository
                    .GetInvestorStatsByUserIdsAsync(investorUserIds);

                foreach (var user in users.Where(u => u.Role == nameof(UserRole.Investor)))
                {
                    if (investorStats.TryGetValue(user.UserId, out var stats))
                    {
                        user.InvestmentCount = stats.InvestmentCount;
                        user.TotalInvested = stats.TotalInvested;
                    }
                    else
                    {
                        user.InvestmentCount = 0;
                        user.TotalInvested = 0;
                    }
                }
            }
        }
    }
}
