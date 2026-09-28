using XFundingHub.Application.Services;
using XFundingHub.Domain.Services;

namespace XFundingHub.UnitTests;

public class LoanApplicationServiceTests
{
    [Fact]
    public void Create_ShouldGenerateApplicationId()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var service = new LoanApplicationService(idGenerator);

        // Act
        var application = service.Create(
            "C1001",
            10000m,
            "GBP",
            12);

        // Assert
        Assert.Equal("LA1001", application.ApplicationId);
    }

    [Fact]
    public void Create_ShouldGenerateIncrementingApplicationIds()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var service = new LoanApplicationService(idGenerator);

        // Act
        var firstApplication = service.Create(
            "C1001",
            10000m,
            "GBP",
            12);

        var secondApplication = service.Create(
            "C1002",
            20000m,
            "GBP",
            24);

        // Assert
        Assert.Equal("LA1001", firstApplication.ApplicationId);
        Assert.Equal("LA1002", secondApplication.ApplicationId);
    }
}