using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Investments.Commands.CreateInvestment;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Moq;
using Xunit;

namespace Investry.Tests;

public class InvestmentTests
{
    [Theory]
    [InlineData(ProjectStatus.PendingReview, -1, 1)]
    [InlineData(ProjectStatus.Rejected, -1, 1)]
    [InlineData(ProjectStatus.FundingClosed, -1, 1)]
    [InlineData(ProjectStatus.Published, -2, -1)]
    [InlineData(ProjectStatus.Published, 1, 2)]
    public async Task ClosedOrUnavailableCampaignRejectsInvestment(ProjectStatus status, int startDays, int endDays)
    {
        var project = new Project
        {
            ProjectStatus = status,
            StartDate = DateTime.UtcNow.AddDays(startDays),
            EndDate = DateTime.UtcNow.AddDays(endDays)
        };
        var repository = new Mock<IProjectRepository>();
        repository.Setup(r => r.GetProjectWithInvestmentDataAsync(project.Id)).ReturnsAsync(project);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.ProjectRepository).Returns(repository.Object);
        var identity = new Mock<IIdentityService>();
        identity.Setup(i => i.GetUserByIdAsync("investor")).ReturnsAsync(new UserDto { KycStatus = KycStatus.Pending });
        var handler = new CreateInvestmentHandler(unitOfWork.Object, identity.Object, Mock.Of<IEmailService>());

        var response = await handler.Handle(new CreateInvestmentCommand(project.Id, "investor", 50), default);

        Assert.True(response.IsFailure);
        Assert.Equal("Project.NotOpen", response.Errors.Single().Code);
        Assert.Equal(0, project.CurrentAmount);
        Assert.Empty(project.Investments);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public async Task NonpositiveInvestmentAmountIsRejected(decimal amount)
    {
        var identity = new Mock<IIdentityService>();
        identity.Setup(i => i.GetUserByIdAsync("investor")).ReturnsAsync(new UserDto { KycStatus = KycStatus.Pending });
        var handler = new CreateInvestmentHandler(Mock.Of<IUnitOfWork>(), identity.Object, Mock.Of<IEmailService>());

        var response = await handler.Handle(new CreateInvestmentCommand(Guid.NewGuid(), "investor", amount), default);

        Assert.True(response.IsFailure);
        Assert.Equal("Investment.InvalidAmount", response.Errors.Single().Code);
    }
}
