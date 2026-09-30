using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Admin.Queries.GetAdminUsers;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminUserDetails
{
    public class GetAdminUserDetailsHandler
        : IRequestHandler<GetAdminUserDetailsQuery, Result<AdminUserDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminUserDetailsHandler(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AdminUserDto>> Handle(
            GetAdminUserDetailsQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetAdminUserByIdAsync(request.UserId);

            if (user is null)
                return Result<AdminUserDto>.Failure(new List<Error>
                {
                    new("User.NotFound", "User not found.", ErrorType.NotFound)
                });

            if (user.Role == nameof(UserRole.Founder))
            {
                var stats = await _unitOfWork.FounderRepository
                    .GetFounderStatsByUserIdsAsync(new List<string> { user.UserId });

                if (stats.TryGetValue(user.UserId, out var founderStats))
                {
                    user.ProjectCount = founderStats.ProjectCount;
                    user.TotalRaised = founderStats.TotalRaised;
                }
                else
                {
                    user.ProjectCount = 0;
                    user.TotalRaised = 0;
                }
            }
            else if (user.Role == nameof(UserRole.Investor))
            {
                var stats = await _unitOfWork.InvestorRepository
                    .GetInvestorStatsByUserIdsAsync(new List<string> { user.UserId });

                if (stats.TryGetValue(user.UserId, out var investorStats))
                {
                    user.InvestmentCount = investorStats.InvestmentCount;
                    user.TotalInvested = investorStats.TotalInvested;
                }
                else
                {
                    user.InvestmentCount = 0;
                    user.TotalInvested = 0;
                }
            }

            return Result<AdminUserDto>.Success(user);
        }
    }
}
