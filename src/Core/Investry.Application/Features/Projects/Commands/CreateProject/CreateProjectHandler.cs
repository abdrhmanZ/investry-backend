using Investry.Application.Common;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Commands.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaService _mediaService;

        public CreateProjectHandler(IUnitOfWork unitOfWork, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mediaService = mediaService;
        }

        public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateProjectCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new Error(
                    e.PropertyName,
                    e.ErrorMessage,
                    ErrorType.Validation))
                    .ToList();

                return Result<Guid>.Failure(errors);
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user is null) return Result<Guid>.Failure(new List<Error> { new Error("UserNotFound", "User not found.", ErrorType.NotFound) });
            if (user.KycStatus != KycStatus.Approved) return Result<Guid>.Failure(new List<Error> { new Error("KYC_Not_Approved", "User KYC not approved.", ErrorType.Validation) });

            //if (request.FundingModel != FundingModel.Reward)
            //    throw new Exception("Only Reward model is supported now");

            var founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);

            var existingProject = await _unitOfWork.ProjectRepository
                .GetByTitleAndFounderIdAsync(request.Title, founder.Id);
            if (existingProject is not null)
                return Result<Guid>.Failure(new List<Error>
                {
                    new Error(
                        "DuplicateProject",
                        "A project with this title already exists for this founder.",
                        ErrorType.Validation)
                });

            var endDate = request.StartDate.AddDays(request.CampaignDurationInDays);

            var project = new Project
            {
                FounderId = founder.Id,
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                LongDescription = request.LongDescription,
                FundingModel = request.FundingModel,
                TargetAmount = request.TargetAmount,
                MinimumContribution = request.MinimumContribution!.Value,
                StartDate = request.StartDate,
                EndDate = endDate,
                Location = request.Location,
                PromotionalVideoURL = request.PromotionalVideoURL,
                ProjectStatus = ProjectStatus.PendingReview,
                CreatedAt = DateTime.UtcNow
            };

            if (request.CategoryIds.Any())
            {
                var categories = await _unitOfWork.CategoryRepository.GetByIdsAsync(request.CategoryIds);

                foreach (var category in categories)
                {
                    project.Categories.Add(category);
                }
            }

            if (request.CoverImage != null)
            {
                var (url, publicId, type) = await _mediaService.AddMediaAsync(request.CoverImage);
                project.Media.Add(new ProjectMedia { MediaUrl = url, PublicId = publicId, Type = type, IsCover = true });
            }

            if (request.MediaFiles != null)
            {
                foreach (var file in request.MediaFiles)
                {
                    var (url, publicId, type) = await _mediaService.AddMediaAsync(file);
                    project.Media.Add(new ProjectMedia { MediaUrl = url, PublicId = publicId, Type = type, IsCover = false });
                }
            }

            switch (request.FundingModel)
            {
                case FundingModel.Reward:
                    HandleReward(project, request);
                    break;

                case FundingModel.Equity:
                    HandleEquity(project, request);
                    break;

                case FundingModel.Mudarabah:
                    HandleMudarabah(project, request);
                    break;

                default:
                    throw new Exception("Unknown FundingModel");
            }

            await _unitOfWork.ProjectRepository.AddAsync(project);

            await _unitOfWork.SaveAsync();

            return Result<Guid>.Success(project.Id);
        }
        private void HandleReward(Project project, CreateProjectCommand request)
        {
            // (Validation already handled by FluentValidation)

            var rewardConfig = new RewardConfig { ProjectId = project.Id };

            if (request.RewardTiers != null)
            {
                foreach (var tier in request.RewardTiers!)
                {
                    rewardConfig.RewardTiers.Add(new RewardTier
                    {
                        Id = Guid.NewGuid(),
                        Title = tier.Title,
                        Description = tier.Description,
                        Amount = tier.Amount,
                        MaxBackers = tier.MaxBackers
                    });
                }
            }

            project.RewardConfig = rewardConfig;
        }
        private void HandleEquity(Project project, CreateProjectCommand request)
        {
            // (Validation already handled by FluentValidation)
            var equityConfig = new EquityConfig
            {
                ProjectId = project.Id,
                EquityPercentage = request.EquityPercentage!.Value
            };

            project.EquityConfig = equityConfig;
        }
        private void HandleMudarabah(Project project, CreateProjectCommand request)
        {
            // (Validation already handled by FluentValidation)
            var mudarabahConfig = new MudarabahConfig
            {
                ProjectId = project.Id,
                InvestorsProfitSharePercentage = request.InvestorsProfitSharePercentage!.Value,
                DurationInMonths = request.DurationInMonths!.Value,
                ProfitDistributionFrequency = request.ProfitDistributionFrequency!.Value
            };

            project.MudarabahConfig = mudarabahConfig;
        }
    }
}

