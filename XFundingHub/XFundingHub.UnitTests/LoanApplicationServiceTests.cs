using XFundingHub.Application.Services;
using XFundingHub.UnitTests.TestDoubles;

namespace XFundingHub.UnitTests;

public class LoanApplicationServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldGenerateApplicationId()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

        // Act
        var application = await service.CreateAsync(
            "C1001",
            10000m,
            "GBP",
            12);

        // Assert
        Assert.Equal("LA1001", application.ApplicationId);
    }

    [Fact]
    public async Task CreateAsync_ShouldGenerateIncrementingApplicationIds()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

        // Act
        var firstApplication = await service.CreateAsync(
            "C1001",
            10000m,
            "GBP",
            12);

        var secondApplication = await service.CreateAsync(
            "C1002",
            20000m,
            "GBP",
            24);

        // Assert
        Assert.Equal("LA1001", firstApplication.ApplicationId);
        Assert.Equal("LA1002", secondApplication.ApplicationId);
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveLoanApplication()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

        // Act
        var application = await service.CreateAsync(
            "C1001",
            10000m,
            "GBP",
            12);

        // Assert
        var savedApplication = await repository.GetByIdAsync(
            application.ApplicationId);

        Assert.NotNull(savedApplication);
        Assert.Equal(
            application.ApplicationId,
            savedApplication.ApplicationId);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidAmount_ShouldNotSaveApplication()
    {
        // Arrange
        var idGenerator = new ApplicationIdGenerator();
        var repository = new FakeLoanApplicationRepository();

        var service = new LoanApplicationService(
            idGenerator,
            repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.CreateAsync(
                "C1001",
                5000m,
                "GBP",
                12));

        // Assert
        var savedApplication =
            await repository.GetByIdAsync("LA1001");

        Assert.Null(savedApplication);
    }
}