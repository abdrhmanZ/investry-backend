using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.CapitalReturns.Commands.ReturnCapital
{
    public class ReturnCapitalHandler : IRequestHandler<ReturnCapitalCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReturnCapitalHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(ReturnCapitalCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectWithInvestmentDataAsync(request.ReturnCapitalRequest.ProjectId);

            if (project == null || project.MudarabahConfig == null)
                return Result<bool>.Failure(new List<Error> { new Error("Project_NotFound", "Mudarabah project not found", ErrorType.NotFound) });

            var founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);

            if (founder == null)
                return Result<bool>.Failure(new List<Error> { new Error("Founder_NotFound", "Founder not found", ErrorType.NotFound) });

            if (project.FounderId != founder.Id)
                return Result<bool>.Failure(new List<Error> { new Error("Unauthorized", "You are not authorized to return capital for this project", ErrorType.Forbidden) });

            if (project.ProjectStatus == ProjectStatus.Completed)
                return Result<bool>.Failure(new List<Error> { new Error("Project Ended", "Project alredy completed and capital was returned.", ErrorType.Validation) });

            var investments = await _unitOfWork.InvestmentRepository.GetByProjectIdAsync(request.ReturnCapitalRequest.ProjectId);

            foreach(var investment in investments)
            {

                var capitalToReturn = new CapitalReturn
                {
                    InvestorId = investment.InvestorId,
                    InvestmentId = investment.Id,
                    MudarabahConfigId = project.MudarabahConfig.Id,
                    OriginalAmount = investment.Amount,
                    CapitalReturnStatus = CapitalReturnStatus.Returned,
                    ReturnedAt = DateTime.UtcNow,
                };
                await _unitOfWork.CapitalReturnRepository.AddAsync(capitalToReturn);

                var investor = await _unitOfWork.InvestorRepository.GetByIdAsync(investment.InvestorId);
                //investor.WalletBalance += investment.Amount;
            }

            project.ProjectStatus = ProjectStatus.Completed;

            await _unitOfWork.SaveAsync();

            return Result<bool>.Success(true);
        }
    }
}
