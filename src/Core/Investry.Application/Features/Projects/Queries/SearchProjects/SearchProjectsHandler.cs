using Investry.Application.Common;
using Investry.Application.Common.Models;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using MediatR;

namespace Investry.Application.Features.Projects.Queries.SearchProjects
{
    public class SearchProjectsHandler : IRequestHandler<SearchProjectsQuery, Result<PagedResult<ProjectSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public SearchProjectsHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<PagedResult<ProjectSummaryDto>>> Handle(SearchProjectsQuery request, CancellationToken cancellationToken)
        {
            var validator = new SearchProjectsValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(error => new Error(error.PropertyName, error.ErrorMessage, ErrorType.Validation))
                    .ToList();
                return Result<PagedResult<ProjectSummaryDto>>.Failure(errors);
            }

            var projectPage = await _unitOfWork.ProjectRepository.SearchProjectsAsync(request, cancellationToken);
            await ProjectSummaryEnricher.PopulateAsync(projectPage.Items, _unitOfWork, _identityService);
            return Result<PagedResult<ProjectSummaryDto>>.Success(projectPage);
        }
    }
}
