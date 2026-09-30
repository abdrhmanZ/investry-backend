using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Investments.Queries.GetMyInvestments
{
    public class GetMyInvestmentsHandler
        : IRequestHandler<GetMyInvestmentsQuery, Result<IReadOnlyList<InvestorInvestmentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyInvestmentsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<InvestorInvestmentDto>>> Handle(
            GetMyInvestmentsQuery request, CancellationToken cancellationToken)
        {
            var investor = await _unitOfWork.InvestorRepository.GetByUserIdAsync(request.UserId);

            if (investor is null)
                return Result<IReadOnlyList<InvestorInvestmentDto>>
                    .Success(new List<InvestorInvestmentDto>().AsReadOnly());

            var investments = await _unitOfWork.InvestmentRepository
                .GetInvestmentsByInvestorIdAsync(investor.Id);

            var dtos = investments.Select(MapToDto).ToList();

            return Result<IReadOnlyList<InvestorInvestmentDto>>.Success(dtos.AsReadOnly());
        }

        private static InvestorInvestmentDto MapToDto(Investment investment)
        {
            var project = investment.Project;

            var dto = new InvestorInvestmentDto
            {
                Id = investment.Id,
                Amount = investment.Amount,
                InvestmentDate = investment.CreatedAt,

                ProjectId = project.Id,
                ProjectTitle = project.Title,
                ProjectCoverImageUrl = GetCoverImageUrl(project),
                ProjectCategory = project.Categories.FirstOrDefault()?.Name ?? string.Empty,
                FundingModel = project.FundingModel.ToString(),
                ProjectTargetAmount = project.TargetAmount,
                ProjectCurrentAmount = project.CurrentAmount,
                ProjectEndDate = project.EndDate
            };

            MapFundingModelDetails(dto, investment);

            return dto;
        }

        private static string? GetCoverImageUrl(Project project)
        {
            return project.Media
                .FirstOrDefault(m => m.IsCover && !m.IsDeleted)?
                .MediaUrl;
        }

        private static void MapFundingModelDetails(InvestorInvestmentDto dto, Investment investment)
        {
            switch (investment.Project.FundingModel)
            {
                case FundingModel.Reward:
                    MapRewardDetails(dto, investment);
                    break;

                case FundingModel.Equity:
                    MapEquityDetails(dto, investment);
                    break;

                case FundingModel.Mudarabah:
                    MapMudarabahDetails(dto, investment);
                    break;
            }
        }

        private static void MapRewardDetails(InvestorInvestmentDto dto, Investment investment)
        {
            if (investment.RewardTier is null) return;

            dto.RewardTierTitle = investment.RewardTier.Title;
            dto.RewardDescription = investment.RewardTier.Description;
            dto.RewardTierAmount = investment.RewardTier.Amount;
        }

        private static void MapEquityDetails(InvestorInvestmentDto dto, Investment investment)
        {
            var share = investment.Investor.InvestorShares
                .FirstOrDefault(s => s.InvestmentId == investment.Id && !s.IsDeleted);

            dto.EquityPercentageOwned = share?.SharesPercentage;
        }

        private static void MapMudarabahDetails(InvestorInvestmentDto dto, Investment investment)
        {
            var mudarabah = investment.Project.MudarabahConfig;
            if (mudarabah is null) return;

            dto.ProfitSharePercentage = mudarabah.InvestorsProfitSharePercentage;

            dto.TotalProfitReceived = investment.Investor.ProfitAllocations
                .Where(pa => pa.InvestmentId == investment.Id && !pa.IsDeleted)
                .Sum(pa => pa.AllocatedProfit);

            dto.CapitalReturned = investment.Investor.CapitalReturns
                .Where(cr => cr.InvestmentId == investment.Id && !cr.IsDeleted)
                .Sum(cr => cr.ReturnedAmount);

            dto.NextPayoutDate = CalculateNextPayoutDate(mudarabah);
        }

        private static DateTime? CalculateNextPayoutDate(MudarabahConfig config)
        {
            if (config.MudarabahStartDate is null)
                return null;

            var frequencyMonths = (int)config.ProfitDistributionFrequency;
            var now = DateTime.UtcNow;
            var nextDate = config.MudarabahStartDate.Value;

            while (nextDate <= now)
                nextDate = nextDate.AddMonths(frequencyMonths);

            if (config.MudarabahEndDate.HasValue && nextDate > config.MudarabahEndDate.Value)
                return null;

            return nextDate;
        }
    }
}
