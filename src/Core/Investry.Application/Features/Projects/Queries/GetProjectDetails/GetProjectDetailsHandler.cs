using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Projects.Commands.CreateProject;
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
                return Result<ProjectDetailsDto>.Failure(new List<Error> { new Error("ProjectNotFound", "The specified project was not found.", ErrorType.NotFound) });

            var founder = await _unitOfWork.FounderRepository.GetByIdAsync(project.FounderId);
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
    }
}
