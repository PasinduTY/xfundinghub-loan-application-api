using XFundingHub.Application.Services;

public class ApplicationIdGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnFirstApplicationId()
    {
        // Arrange
        var generator = new ApplicationIdGenerator();

        // Act
        var applicationId = generator.Generate(1001);

        // Assert
        Assert.Equal("LA1001", applicationId);
    }

    [Fact]
    public void Generate_ShouldIncrementApplicationId()
    {
        // Arrange
        var generator = new ApplicationIdGenerator();

        // Act
        var firstId = generator.Generate(1001);
        var secondId = generator.Generate(1002);
        var thirdId = generator.Generate(1003);

        // Assert
        Assert.Equal("LA1001", firstId);
        Assert.Equal("LA1002", secondId);
        Assert.Equal("LA1003", thirdId);
    }
}
