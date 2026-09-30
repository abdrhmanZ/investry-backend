using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.ActivateMudarabah
{
    public class ActivateMudarabahHandler : IRequestHandler<ActivateMudarabahCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivateMudarabahHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(ActivateMudarabahCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectWithInvestmentDataAsync(request.ProjectId);

            if (project is null)
                return Result<bool>.Failure(new List<Error> { new Error("Project_NotFound", "Project not found", ErrorType.NotFound) });

            var founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);
            if (founder is null || project.FounderId != founder.Id)
                return Result<bool>.Failure(new List<Error> { new Error("Authorization.Forbidden", "You are not authorized to activate this project.", ErrorType.Unauthorized) });

            if (project.FundingModel != FundingModel.Mudarabah)
                return Result<bool>.Failure(new List<Error> { new Error("Invalid_FundingModel", "Project is not a Mudarabah project", ErrorType.Validation) });

            if (project.MudarabahConfig == null)
                return Result<bool>.Failure(new List<Error> { new Error("Config_NotFound", "Mudarabah configuration not found", ErrorType.NotFound) });

            if (project.CurrentAmount < project.TargetAmount)
                return Result<bool>.Failure(new List<Error> { new Error("Not_Funded", "Project has not reached target amount yet", ErrorType.Validation) });

            if (project.MudarabahConfig.MudarabahStartDate != null)
                return Result<bool>.Failure(new List<Error> { new Error("Already_Activated", "Mudarabah project already activated", ErrorType.Validation) });

            var mudarabahConfig = project.MudarabahConfig;

            mudarabahConfig.MudarabahStartDate = DateTime.UtcNow;
            mudarabahConfig.MudarabahEndDate = DateTime.UtcNow.AddMonths(mudarabahConfig.DurationInMonths);

            project.ProjectStatus = ProjectStatus.Published;

            await _unitOfWork.SaveAsync();

            return Result<bool>.Success(true);
        }
    }
}
