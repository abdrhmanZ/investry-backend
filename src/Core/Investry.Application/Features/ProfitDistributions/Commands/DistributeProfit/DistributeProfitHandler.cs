using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.ProfitDistributions.Commands.DistributeProfit
{
    public class DistributeProfitHandler : IRequestHandler<DistributeProfitCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DistributeProfitHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(DistributeProfitCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectWithInvestmentDataAsync(request.DistributeProfitRequest.ProjectId);

            if (project is null || project.MudarabahConfig is null)
                return Result<Guid>.Failure(new List<Error> { new Error("Project_NotFound", "Project not found or not configured for profit distribution", ErrorType.NotFound) });

            var founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);
            if (founder == null)
                return Result<Guid>.Failure(new List<Error> { new Error("Founder_NotFound", "Founder not found", ErrorType.NotFound) });

            if (project.FounderId != founder.Id)
                return Result<Guid>.Failure(new List<Error> { new Error("Unauthorized", "You are not authorized to distribute profit for this project", ErrorType.Forbidden) });

            if (project.MudarabahConfig.MudarabahStartDate == null)
                return Result<Guid>.Failure(new List<Error> { new Error("Not_Activated", "Mudarabah project not activated yet", ErrorType.Validation) });

            var existingDistribution = await _unitOfWork.ProfitDistributionRepository
        .ExistsForPeriodAsync(request.DistributeProfitRequest.ProjectId, request.DistributeProfitRequest.PeriodStart, request.DistributeProfitRequest.PeriodEnd);

            if (existingDistribution)
                return Result<Guid>.Failure(new List<Error>{ new Error("Duplicate_Distribution",
                $"Profit distribution already exists for the period from {request.DistributeProfitRequest.PeriodStart:yyyy-MM-dd} to {request.DistributeProfitRequest.PeriodEnd:yyyy-MM-dd}",
                ErrorType.Validation) });

            var config = project.MudarabahConfig;

            decimal investorsPoolProfit = request.DistributeProfitRequest.NetProfit * (config.InvestorsProfitSharePercentage / 100m);
            decimal founderProfit = request.DistributeProfitRequest.NetProfit * (config.FounderProfitSharePercentage / 100m);

            var profitDistribution = new ProfitDistribution
            {
                MudarabahConfigId = config.Id,
                PeriodStart = request.DistributeProfitRequest.PeriodStart,
                PeriodEnd = request.DistributeProfitRequest.PeriodEnd,
                NetProfit = request.DistributeProfitRequest.NetProfit,
                InvestorsPoolProfit = investorsPoolProfit,
                FounderProfit = founderProfit,
                DistributionStatus = DistributionStatus.Pending,
            };
            await _unitOfWork.ProfitDistributionRepository.AddAsync(profitDistribution);

            var investments = await _unitOfWork.InvestmentRepository.GetByProjectIdAsync(project.Id);

            foreach(var investment in investments)
            {
                decimal allcatedProfit = investorsPoolProfit * investment.CapitalRatio!.Value;

                var allocation = new InvestorProfitAllocation
                {
                    ProfitDistributionId = profitDistribution.Id,
                    InvestorId = investment.InvestorId,
                    InvestmentId = investment.Id,
                    CapitalRatio = investment.CapitalRatio!.Value,
                    AllocatedProfit = allcatedProfit
                };

                await _unitOfWork.InvestorProfitAllocationRepository.AddAsync(allocation);

                var investor = await _unitOfWork.InvestorRepository.GetByIdAsync(investment.InvestorId);
                //investor.WalletBalance += allcatedProfit;
            }

            // لما نعمل محفظة للمؤسس
            //var founder = await _unitOfWork.FounderRepository.GetByIdAsync(project.Id);
            //founder.WalletBalance += founderProfit;

            profitDistribution.DistributionStatus = DistributionStatus.Distributed;
            profitDistribution.DistributedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();

            return Result<Guid>.Success(profitDistribution.Id);
        }
    }
}
