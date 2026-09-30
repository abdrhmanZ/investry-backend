using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Enums;
using MediatR;

namespace Investry.Application.Features.Admin.Queries.GetAdminProjects
{
    public class GetAdminProjectsHandler
        : IRequestHandler<GetAdminProjectsQuery, Result<IReadOnlyList<AdminProjectDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;

        public GetAdminProjectsHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Result<IReadOnlyList<AdminProjectDto>>> Handle(
            GetAdminProjectsQuery request, CancellationToken cancellationToken)
        {
            var status = ParseStatus(request.Status);

            var projects = await _unitOfWork.ProjectRepository
                .GetAdminProjectsAsync(status);

            if (!projects.Any())
                return Result<IReadOnlyList<AdminProjectDto>>.Success(projects);

            await PopulateFounderInfo(projects);

            if (!string.IsNullOrWhiteSpace(request.Search))
                projects = FilterBySearch(projects, request.Search);

            return Result<IReadOnlyList<AdminProjectDto>>.Success(projects);
        }

        private static ProjectStatus? ParseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            return Enum.TryParse<ProjectStatus>(status, ignoreCase: true, out var parsed)
                ? parsed
                : null;
        }

        private async Task PopulateFounderInfo(IReadOnlyList<AdminProjectDto> projects)
        {
            var founderIds = projects.Select(p => p.FounderId).Distinct().ToList();

            var founders = await _unitOfWork.FounderRepository.GetByFounderIdsAsync(founderIds);
            var userIds = founders.Select(f => f.UserId).ToList();

            var userInfoDictionary = await _identityService.GetUserInfoByIdsAsync(userIds);

            foreach (var project in projects)
            {
                var founder = founders.FirstOrDefault(f => f.Id == project.FounderId);
                if (founder is null || !userInfoDictionary.TryGetValue(founder.UserId, out var info))
                    continue;

                project.FounderName = info.Name;
                project.FounderEmail = info.Email;
            }
        }

        private static IReadOnlyList<AdminProjectDto> FilterBySearch(
            IReadOnlyList<AdminProjectDto> projects, string search)
        {
            return projects
                .Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                         || p.FounderName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .AsReadOnly();
        }
    }
}
