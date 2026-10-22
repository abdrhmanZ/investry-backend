using Investry.Application.Common;
using Investry.Application.Common.Models;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Projects.Queries.GetAllProjects;
using Investry.Application.Features.Projects.Queries.SearchProjects;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Moq;
using Xunit;

namespace Investry.Tests;

public class ProjectSearchTests
{
    [Theory]
    [InlineData(0, 12, null, 0, "Page")]
    [InlineData(-1, 12, null, 0, "Page")]
    [InlineData(1, 0, null, 0, "PageSize")]
    [InlineData(1, -1, null, 0, "PageSize")]
    [InlineData(1, 51, null, 0, "PageSize")]
    [InlineData(int.MaxValue, 50, null, 0, "Page")]
    [InlineData(1, 12, 999, 0, "FundingModel")]
    [InlineData(1, 12, null, 201, "Search")]
    public async Task InvalidSearchRequestReturnsValidationFailure(
        int page, int pageSize, int? fundingModel, int searchLength, string errorCode)
    {
        var request = new SearchProjectsQuery
        {
            Page = page,
            PageSize = pageSize,
            FundingModel = (FundingModel?)fundingModel,
            Search = searchLength > 0 ? new string('x', searchLength) : null
        };
        var handler = new SearchProjectsHandler(Mock.Of<IUnitOfWork>(), Mock.Of<IIdentityService>());

        var response = await handler.Handle(request, default);

        Assert.True(response.IsFailure);
        Assert.Contains(response.Errors, error => error.Code == errorCode && error.Type == ErrorType.Validation);
    }

    [Fact]
    public async Task SearchWithoutMatchesReturnsAnEmptyPage()
    {
        var repository = new Mock<IProjectRepository>();
        repository.Setup(r => r.SearchProjectsAsync(It.IsAny<SearchProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<ProjectSummaryDto>(Array.Empty<ProjectSummaryDto>(), 0, 1, 12));
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.ProjectRepository).Returns(repository.Object);
        var handler = new SearchProjectsHandler(unitOfWork.Object, Mock.Of<IIdentityService>());

        var response = await handler.Handle(new SearchProjectsQuery { Search = "no matches" }, default);

        Assert.True(response.IsSuccess);
        Assert.Empty(response.Value.Items);
        Assert.Equal(0, response.Value.TotalCount);
        Assert.Equal(0, response.Value.TotalPages);
        Assert.False(response.Value.HasNextPage);
        Assert.False(response.Value.HasPreviousPage);
    }

    [Fact]
    public async Task SearchPageIncludesFounderNamesAndRemainingDaysWithoutLosingPagination()
    {
        var founder = new Founder { UserId = "founder-user" };
        var founderWithoutName = new Founder { UserId = "missing-user" };
        var now = DateTime.UtcNow;
        var projects = new[]
        {
            new ProjectSummaryDto { FounderId = founder.Id, Title = "Open project", EndDate = now.AddHours(60) },
            new ProjectSummaryDto { FounderId = founder.Id, Title = "Ended project", EndDate = now.AddDays(-2) },
            new ProjectSummaryDto { FounderId = Guid.NewGuid(), Title = "Missing founder", EndDate = now.AddHours(36) },
            new ProjectSummaryDto { FounderId = founderWithoutName.Id, Title = "Missing user", EndDate = now.AddDays(-1) }
        };
        var repository = new Mock<IProjectRepository>();
        repository.Setup(r => r.SearchProjectsAsync(It.IsAny<SearchProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<ProjectSummaryDto>(projects, 13, 2, 4));
        var founders = new Mock<IFounderRepository>();
        founders.Setup(r => r.GetByFounderIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<Founder> { founder, founderWithoutName });
        var identity = new Mock<IIdentityService>();
        identity.Setup(i => i.GetUserNamesByIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Dictionary<string, string> { [founder.UserId] = "Mona Hassan" });
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.ProjectRepository).Returns(repository.Object);
        unitOfWork.SetupGet(u => u.FounderRepository).Returns(founders.Object);
        var handler = new SearchProjectsHandler(unitOfWork.Object, identity.Object);

        var response = await handler.Handle(new SearchProjectsQuery { Page = 2, PageSize = 4 }, default);

        Assert.True(response.IsSuccess);
        var page = response.Value;
        Assert.Equal(13, page.TotalCount);
        Assert.Equal(2, page.Page);
        Assert.Equal(4, page.PageSize);
        Assert.Equal(4, page.TotalPages);
        Assert.True(page.HasNextPage);
        Assert.True(page.HasPreviousPage);
        Assert.Collection(page.Items,
            project =>
            {
                Assert.Equal("Open project", project.Title);
                Assert.Equal("Mona Hassan", project.FounderName);
                Assert.Equal(3, project.DaysRemaining);
            },
            project =>
            {
                Assert.Equal("Ended project", project.Title);
                Assert.Equal("Mona Hassan", project.FounderName);
                Assert.Equal(0, project.DaysRemaining);
            },
            project =>
            {
                Assert.Equal("Missing founder", project.Title);
                Assert.Null(project.FounderName);
                Assert.Equal(2, project.DaysRemaining);
            },
            project =>
            {
                Assert.Equal("Missing user", project.Title);
                Assert.Null(project.FounderName);
                Assert.Equal(0, project.DaysRemaining);
            });
    }
}
