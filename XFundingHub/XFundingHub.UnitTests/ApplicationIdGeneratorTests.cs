using XFundingHub.Application.Services;

public class ApplicationIdGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnFirstApplicationId()
    {
        // Arrange
        var generator = new ApplicationIdGenerator();

        // Act
        var applicationId = generator.Generate();

        // Assert
        Assert.Equal("LA1001", applicationId);
    }

    [Fact]
    public void Generate_ShouldIncrementApplicationId()
    {
        // Arrange
        var generator = new ApplicationIdGenerator();

        // Act
        var firstId = generator.Generate();
        var secondId = generator.Generate();
        var thirdId = generator.Generate();

        // Assert
        Assert.Equal("LA1001", firstId);
        Assert.Equal("LA1002", secondId);
        Assert.Equal("LA1003", thirdId);
    }
}
