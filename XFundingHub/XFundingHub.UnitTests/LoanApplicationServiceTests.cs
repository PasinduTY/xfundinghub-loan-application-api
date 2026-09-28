using XFundingHub.Application.Services;
using XFundingHub.UnitTests.TestDoubles;

namespace XFundingHub.UnitTests;

public class LoanApplicationServiceTests
{
    [Fact]
    public void Create_ShouldGenerateApplicationId()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

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
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

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

    [Fact]
    public void Create_ShouldSaveLoanApplication()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

        // Act
        var application = service.Create(
            "C1001",
            10000m,
            "GBP",
            12);

        // Assert
        var savedApplication = repository.GetById(application.ApplicationId);

        Assert.NotNull(savedApplication);
        Assert.Equal(application.ApplicationId, savedApplication.ApplicationId);
    }

    [Fact]
    public void Create_WithInvalidAmount_ShouldNotSaveApplication()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Create(
                "C1001",
                5000m,
                "GBP",
                12));

        // Assert
        var savedApplication = repository.GetById("LA1001");

        Assert.Null(savedApplication);
    }
}