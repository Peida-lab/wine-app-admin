using WineApp.Domain.Wines.ValueObjects;

namespace WineApp.Test.Domain.Wines.ValueObjects;

public class WinePercentageTests
{
    [Fact]
    public void Constructor_ShouldSetValue_WhenPercentageIsValid()
    {
        // Arrange
        decimal percentage = 13.5m;

        // Act
        WinePercentage winePercentage = new(percentage);

        // Assert
        Assert.Equal(percentage, winePercentage.Value);
    }

    [Fact]
    public void Constructor_ShouldAllowEightPercent()
    {
        // Arrange
        decimal percentage = 8m;

        // Act
        WinePercentage winePercentage = new(percentage);

        // Assert
        Assert.Equal(percentage, winePercentage.Value);
    }

    [Fact]
    public void Constructor_ShouldAllowTwentyPercent()
    {
        // Arrange
        decimal percentage = 20m;

        // Act
        WinePercentage winePercentage = new(percentage);

        // Assert
        Assert.Equal(percentage, winePercentage.Value);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenPercentageIsBelowEight()
    {
        // Arrange
        decimal percentage = 7.9m;

        // Act
        Action action = () => new WinePercentage(percentage);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenPercentageIsAboveTwenty()
    {
        // Arrange
        decimal percentage = 20.1m;

        // Act
        Action action = () => new WinePercentage(percentage);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }
}