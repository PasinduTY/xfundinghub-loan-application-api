using Microsoft.EntityFrameworkCore;
using XFundingHub.Domain.Entities;
using XFundingHub.Infrastructure.Repositories;
using XFundingHub.IntegrationTests.Infrastructure;

namespace XFundingHub.IntegrationTests.Repositories;

public class LoanApplicationRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistLoanApplication()
    {
        // Arrange
        await using var context = IntegrationTestDbContextFactory.Create();

        var repository = new LoanApplicationRepository(context);

        var applicationId = $"TEST-{Guid.NewGuid():N}";

        var application = new LoanApplication(
            applicationId,
            "C9999",
            10000m,
            "GBP",
            12);

        // Act
        await repository.AddAsync(application);

        // Assert
        var savedApplication = await context.LoanApplications
            .FirstOrDefaultAsync(x => x.ApplicationId == applicationId);

        Assert.NotNull(savedApplication);
        Assert.Equal(applicationId, savedApplication.ApplicationId);
        Assert.Equal("C9999", savedApplication.CustomerId);
        Assert.Equal(10000m, savedApplication.Amount);
        Assert.Equal("GBP", savedApplication.Currency);
        Assert.Equal(12, savedApplication.TermMonths);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPersistedLoanApplication()
    {
        // Arrange
        await using var context = IntegrationTestDbContextFactory.Create();

        var repository = new LoanApplicationRepository(context);

        var applicationId = $"TEST-{Guid.NewGuid():N}";

        var application = new LoanApplication(
            applicationId,
            "C9998",
            25000m,
            "GBP",
            24);

        await repository.AddAsync(application);

        // Act
        var result = await repository.GetByIdAsync(applicationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationId, result.ApplicationId);
        Assert.Equal("C9998", result.CustomerId);
        Assert.Equal(25000m, result.Amount);
        Assert.Equal("GBP", result.Currency);
        Assert.Equal(24, result.TermMonths);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenApplicationDoesNotExist()
    {
        // Arrange
        await using var context = IntegrationTestDbContextFactory.Create();

        var repository = new LoanApplicationRepository(context);

        var applicationId = $"TEST-{Guid.NewGuid():N}";

        // Act
        var result = await repository.GetByIdAsync(applicationId);

        // Assert
        Assert.Null(result);
    }
}