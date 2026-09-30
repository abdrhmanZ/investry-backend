using Investry.Application.Contracts.Persistence;
using Investry.Application.Features.Projects.Commands.ActivateMudarabah;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Moq;
using Xunit;

namespace Investry.Tests;

public class MudarabahTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task OnlyTheProjectFounderCanActivateMudarabah(bool owner)
    {
        var founder = new Founder();
        var project = new Project
        {
            FounderId = owner ? founder.Id : Guid.NewGuid(),
            FundingModel = FundingModel.Mudarabah,
            CurrentAmount = 1000,
            TargetAmount = 1000,
            MudarabahConfig = new MudarabahConfig { DurationInMonths = 6 }
        };
        var projects = new Mock<IProjectRepository>();
        projects.Setup(r => r.GetProjectWithInvestmentDataAsync(project.Id)).ReturnsAsync(project);
        var founders = new Mock<IFounderRepository>();
        founders.Setup(r => r.GetByUserIdAsync("founder")).ReturnsAsync(founder);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(u => u.ProjectRepository).Returns(projects.Object);
        unitOfWork.SetupGet(u => u.FounderRepository).Returns(founders.Object);
        unitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        var response = await new ActivateMudarabahHandler(unitOfWork.Object)
            .Handle(new ActivateMudarabahCommand(project.Id, "founder"), default);

        Assert.Equal(owner, response.IsSuccess);
        if (owner)
            Assert.NotNull(project.MudarabahConfig.MudarabahStartDate);
        else
        {
            Assert.Equal("Authorization.Forbidden", response.Errors.Single().Code);
            Assert.Null(project.MudarabahConfig.MudarabahStartDate);
        }
    }
}
