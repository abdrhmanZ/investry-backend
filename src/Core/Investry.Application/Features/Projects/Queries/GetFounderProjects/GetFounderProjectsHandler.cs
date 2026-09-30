using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.GetFounderProjects
{
    public class GetFounderProjectsHandler : IRequestHandler<GetFounderProjectsQuery, Result<IReadOnlyList<FounderProjectDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFounderProjectsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IReadOnlyList<FounderProjectDto>>> Handle(GetFounderProjectsQuery request, CancellationToken cancellationToken)

        {
            var Founder = await _unitOfWork.FounderRepository.GetByUserIdAsync(request.UserId);
            if(Founder == null)
                {
                return Result<IReadOnlyList<FounderProjectDto>>.Failure(new List<Error> { new Error("FounderNotFound", "No founder found for the given user ID.", ErrorType.NotFound) });
            }
            var projects = await _unitOfWork.ProjectRepository.GetProjectsByFounderIdAsync(Founder.Id);

            return Result<IReadOnlyList<FounderProjectDto>>.Success(projects);
        }
    }
}