using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.DeleteProject
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProjectHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectWithDependenciesForDeletionAsync(request.ProjectId);
            if (project is null)
                return Result<bool>.Failure(new List<Error> { new Error("Project.NotFound", "The project with the specified ID was not found.", ErrorType.NotFound) });

            if (project.IsDeleted)
                return Result<bool>.Failure(new List<Error> { new Error("Project.AlreadyDeleted", "The project has already been deleted.", ErrorType.Conflict) });

            var founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);
            if (founder == null || project.FounderId != founder.Id)
                return Result<bool>.Failure(new List<Error> { new Error("Authorization.Forbidden", "You are not authorized to delete this project.", ErrorType.Unauthorized) });

            bool hasInvestments = await _unitOfWork.InvestmentRepository.HasInvestmentsAsync(project.Id);
            if (hasInvestments)
                return Result<bool>.Failure(new List<Error> { new Error("Project.HasInvestments", "Cannot delete a project that has investments.", ErrorType.Conflict) });

            if (project.ProjectStatus != ProjectStatus.PendingReview && project.ProjectStatus != ProjectStatus.Rejected)
                return Result<bool>.Failure(new List<Error> { new Error("roject.IsActive", "Cannot delete a project that is active or has been funded.", ErrorType.Conflict) });

            project.IsDeleted = true;
            project.DeletedAt = DateTime.UtcNow;

            if (project.RewardConfig != null)
            {
                project.RewardConfig.IsDeleted = true;
                foreach (var tier in project.RewardConfig.RewardTiers)
                {
                    tier.IsDeleted = true;
                }
            }
            if (project.EquityConfig != null)
            {
                project.EquityConfig.IsDeleted = true;
                foreach (var share in project.EquityConfig.InvestorShares)
                {
                    share.IsDeleted = true;
                }
            }
            if (project.MudarabahConfig != null)
            {
                project.MudarabahConfig.IsDeleted = true;
                foreach (var distribution in project.MudarabahConfig.ProfitDistributions)
                {
                    distribution.IsDeleted = true;
                    foreach (var allocation in distribution.InvestorProfitAllocations)
                    {
                        allocation.IsDeleted = true;
                    }
                }
                foreach (var returnItem in project.MudarabahConfig.CapitalReturns)
                {
                    returnItem.IsDeleted = true;
                }
            }

            foreach (var media in project.Media)
            {
                media.IsDeleted = true;
            }

            await _unitOfWork.SaveAsync();

            return Result<bool>.Success(true);
        }
    }
}
