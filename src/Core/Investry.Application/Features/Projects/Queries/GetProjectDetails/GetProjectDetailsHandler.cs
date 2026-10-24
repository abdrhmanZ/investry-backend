using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.GetProjectDetails
{
    public class GetProjectDetailsHandler : IRequestHandler<GetProjectDetailsQuery, Result<ProjectDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public GetProjectDetailsHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }
        public async Task<Result<ProjectDetailsDto>> Handle(GetProjectDetailsQuery request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectDetailsAsync(request.ProjectId);

            if (project is null)
                return ProjectNotFound();

            var founder = await _unitOfWork.FounderRepository.GetByIdAsync(project.FounderId);
            var canPreview = request.UserId != null
                && (request.IsAdministrator || founder?.UserId == request.UserId);
            if (!IsPublicProject(project.ProjectStatus) && !canPreview)
                return ProjectNotFound();

            if (founder != null)
            {
                var userNames = await _identityService.GetUserNamesByIdsAsync(new List<string> { founder.UserId });
                if (userNames.TryGetValue(founder.UserId, out var founderName))
                {
                    project.FounderName = founderName;
                }
            }

            return Result<ProjectDetailsDto>.Success(project);
        }

        private static bool IsPublicProject(string status)
            => status is nameof(ProjectStatus.Published) or nameof(ProjectStatus.FundingClosed)
                or nameof(ProjectStatus.Completed) or nameof(ProjectStatus.Successful) or nameof(ProjectStatus.Failed);

        private static Result<ProjectDetailsDto> ProjectNotFound()
            => Result<ProjectDetailsDto>.Failure(new List<Error>
            {
                new Error("ProjectNotFound", "The specified project was not found.", ErrorType.NotFound)
            });
    }
}
