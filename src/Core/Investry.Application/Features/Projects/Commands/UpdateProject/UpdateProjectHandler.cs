using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.UpdateProject
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaService _mediaService;

        public UpdateProjectHandler(IUnitOfWork unitOfWork, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
        }
        public async Task<Result<bool>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectWithDependenciesForUpdateAsync(request.ProjectId);
            if (project is null)
                return Result<bool>.Failure(new List<Error> { new Error("Project.NotFound", "Project not found.", ErrorType.NotFound) });

            var founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);
            if (founder is null || project.FounderId != founder.Id)
                return Result<bool>.Failure(new List<Error> { new Error("Authorization.Forbidden", "You are not authorized to update this project.", ErrorType.Unauthorized) });

            bool hasInvestments = await _unitOfWork.InvestmentRepository.HasInvestmentsAsync(request.ProjectId);
            if (hasInvestments)
            {
                if (IsCoreDataChanged(request))
                    return Result<bool>.Failure(new List<Error> { new Error("Update.Forbidden", "Cannot modify core project details after investments have been made.", ErrorType.Unauthorized) });

                if (request.LongDescription != null) project.LongDescription = request.LongDescription;
            }
            else if (project.ProjectStatus == ProjectStatus.PendingReview || project.ProjectStatus == ProjectStatus.Rejected)
            {
                await UpdateAllProjectProperties(project, request);
                project.ProjectStatus = ProjectStatus.PendingReview;
            }
            else
            {
                return Result<bool>.Failure(new List<Error> { new Error("Update.Forbidden", $"Cannot update project with status '{project.ProjectStatus}'.", ErrorType.Unauthorized) });
            }

            await HandleMediaOperations(project, request);

            await _unitOfWork.SaveAsync();
            
            return Result<bool>.Success(true);
        }

        #region Helper Methods

        private async Task UpdateAllProjectProperties(Project project, UpdateProjectCommand request)
        {
            if (request.Title != null) project.Title = request.Title;
            if (request.ShortDescription != null) project.ShortDescription = request.ShortDescription;
            if (request.LongDescription != null) project.LongDescription = request.LongDescription;
            if (request.Location != null) project.Location = request.Location;
            if (request.TargetAmount.HasValue) project.TargetAmount = request.TargetAmount.Value;
            if (request.MinimumContribution.HasValue) project.MinimumContribution = request.MinimumContribution.Value;
            if (request.DurationInDays.HasValue)
                project.EndDate = project.StartDate.AddDays(request.DurationInDays.Value);

            if (request.CategoryIds != null)
            {
                var newCategories = await _unitOfWork.CategoryRepository.GetByIdsAsync(request.CategoryIds);

                project.Categories = newCategories;
            }

            switch (project.FundingModel)
            {
                case FundingModel.Reward:
                    if (request.RewardTiers != null && project.RewardConfig != null)
                    {
                        await _unitOfWork.RewardTierRepository.DeleteTiersByConfigIdAsync(project.RewardConfig.Id);
                        foreach (var tierDto in request.RewardTiers)
                        {
                            var newTier = new RewardTier
                            {
                                Title = tierDto.Title,
                                Description = tierDto.Description,
                                Amount = tierDto.Amount,
                                MaxBackers = tierDto.MaxBackers,
                                RewardConfigId = project.RewardConfig.Id
                            };
                            await _unitOfWork.RewardTierRepository.AddAsync(newTier);
                        }
                    }
                    break;
                case FundingModel.Equity:
                    if (request.EquityPercentage.HasValue && project.EquityConfig != null)
                    {
                        project.EquityConfig.EquityPercentage = request.EquityPercentage.Value;
                    }
                    break;
                case FundingModel.Mudarabah:
                    if (request.InvestorsProfitSharePercentage.HasValue && project.MudarabahConfig != null)
                    {
                        project.MudarabahConfig.InvestorsProfitSharePercentage = request.InvestorsProfitSharePercentage.Value;
                    }
                    if (request.MudarabahDurationInMonths.HasValue && project.MudarabahConfig != null)
                    {
                        project.MudarabahConfig.DurationInMonths = request.MudarabahDurationInMonths.Value;
                    }
                    if (request.ProfitDistributionFrequency.HasValue && project.MudarabahConfig != null)
                    {
                        project.MudarabahConfig.ProfitDistributionFrequency = request.ProfitDistributionFrequency.Value;
                    }
                    break;
            }
        }

        private async Task HandleMediaOperations(Project project, UpdateProjectCommand request)
        {
            if (request.DeletedMediaPublicIds != null && request.DeletedMediaPublicIds.Any())
            {
                var mediaToDelete = project.Media
                    .Where(m => request.DeletedMediaPublicIds.Contains(m.PublicId))
                    .ToList();

                foreach (var media in mediaToDelete)
                {
                    await _mediaService.DeleteMediaAsync(media.PublicId, media.Type);

                    await _unitOfWork.ProjectMediaRepository.DeleteAsync(media);
                }
            }

            if (request.NewMediaFiles != null && request.NewMediaFiles.Any())
            {
                foreach (var fileDto in request.NewMediaFiles)
                {
                    var (url, publicId, type) = await _mediaService.AddMediaAsync(fileDto);

                    var newMedia = new ProjectMedia
                    {
                        ProjectId = project.Id,
                        MediaUrl = url,
                        PublicId = publicId,
                        Type = type,
                        IsCover = false
                    };

                    await _unitOfWork.ProjectMediaRepository.AddAsync(newMedia);
                }
            }

            if (!string.IsNullOrEmpty(request.NewCoverImagePublicId))
            {
                project.Media.ToList().ForEach(m => m.IsCover = false);
                var newCover = project.Media.FirstOrDefault(m => m.PublicId == request.NewCoverImagePublicId);
                if (newCover != null) newCover.IsCover = true;
            }

        }

        private bool IsCoreDataChanged(UpdateProjectCommand request)
        {
            return request.Title != null ||
                   request.ShortDescription != null ||
                   request.TargetAmount.HasValue ||
                   request.MinimumContribution.HasValue ||
                   request.DurationInDays.HasValue ||
                   request.CategoryIds != null ||
                   request.RewardTiers != null ||
                   request.EquityPercentage.HasValue ||
                   request.InvestorsProfitSharePercentage.HasValue ||
                   request.MudarabahDurationInMonths.HasValue;
        }

        #endregion
    }
}
