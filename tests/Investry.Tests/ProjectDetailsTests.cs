using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Projects.Queries.GetProjectDetails;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Moq;
using Xunit;

namespace Investry.Tests;

public class ProjectDetailsTests
{
    [Theory]
    [InlineData(ProjectStatus.PendingReview, null, false, false)]
    [InlineData(ProjectStatus.PendingReview, null, true, false)]
    [InlineData(ProjectStatus.PendingReview, "another-founder", false, false)]
    [InlineData(ProjectStatus.PendingReview, "owner", false, true)]
    [InlineData(ProjectStatus.PendingReview, "admin", true, true)]
    [InlineData(ProjectStatus.Rejected, null, false, false)]
    [InlineData(ProjectStatus.Rejected, "another-founder", false, false)]
    [InlineData(ProjectStatus.Rejected, "owner", false, true)]
    [InlineData(ProjectStatus.Rejected, "admin", true, true)]
    public async Task UnpublishedProjectIsVisibleOnlyToItsOwnerOrAdministrator(
        ProjectStatus status, string? userId, bool administrator, bool allowed)
    {
        var project = new ProjectDetailsDto { Id = Guid.NewGuid(), FounderId = Guid.NewGuid(), ProjectStatus = status.ToString() };
        var handler = HandlerFor(project);

        var response = await handler.Handle(new GetProjectDetailsQuery(project.Id)
        {
            UserId = userId,
            IsAdministrator = administrator
        }, default);

        Assert.Equal(allowed, response.IsSuccess);
        if (allowed)
        {
            Assert.Equal(project.Id, response.Value.Id);
            Assert.Equal("Project Owner", response.Value.FounderName);
        }
        else
        {
            Assert.Null(response.Value);
            Assert.Equal("ProjectNotFound", response.Errors.Single().Code);
            Assert.Equal(ErrorType.NotFound, response.Errors.Single().Type);
        }
    }

    [Theory]
    [InlineData(ProjectStatus.Published)]
    [InlineData(ProjectStatus.FundingClosed)]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.Successful)]
    [InlineData(ProjectStatus.Failed)]
    public async Task ApprovedProjectRemainsVisibleAfterItsFundingStage(ProjectStatus status)
    {
        var project = new ProjectDetailsDto { Id = Guid.NewGuid(), FounderId = Guid.NewGuid(), ProjectStatus = status.ToString() };

        var response = await HandlerFor(project).Handle(new GetProjectDetailsQuery(project.Id), default);

        Assert.True(response.IsSuccess);
        Assert.Equal(project.Id, response.Value.Id);
        Assert.Equal(status.ToString(), response.Value.ProjectStatus);
    }

    [Fact]
    public async Task MissingProjectReturnsTheSameErrorAsAnInaccessibleProject()
    {
        var missing = await HandlerFor(null).Handle(new GetProjectDetailsQuery(Guid.NewGuid()), default);
        var project = new ProjectDetailsDto { Id = Guid.NewGuid(), FounderId = Guid.NewGuid(), ProjectStatus = nameof(ProjectStatus.PendingReview) };
        var hidden = await HandlerFor(project).Handle(new GetProjectDetailsQuery(project.Id), default);

        Assert.True(missing.IsFailure);
        Assert.Null(missing.Value);
        Assert.Equal(hidden.Errors.Single().Code, missing.Errors.Single().Code);
        Assert.Equal(hidden.Errors.Single().Message, missing.Errors.Single().Message);
        Assert.Equal(ErrorType.NotFound, missing.Errors.Single().Type);
    }

    private static GetProjectDetailsHandler HandlerFor(ProjectDetailsDto? project)
    {
        var projects = new Mock<IProjectRepository>();
        projects.Setup(repository => repository.GetProjectDetailsAsync(It.IsAny<Guid>())).ReturnsAsync(project);
        var founders = new Mock<IFounderRepository>();
        founders.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Founder { Id = project?.FounderId ?? Guid.NewGuid(), UserId = "owner" });
        var identity = new Mock<IIdentityService>();
        identity.Setup(service => service.GetUserNamesByIdsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(new Dictionary<string, string> { ["owner"] = "Project Owner" });
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(unit => unit.ProjectRepository).Returns(projects.Object);
        unitOfWork.SetupGet(unit => unit.FounderRepository).Returns(founders.Object);
        return new GetProjectDetailsHandler(unitOfWork.Object, identity.Object);
    }
}
