namespace WineApp.Domain.Wines.ValueObjects;

public record WinePercentage
{
    public decimal Value { get; }
    public WinePercentage(decimal value)
    {
        if (value < 8 || value > 20)
            throw new ArgumentException("Percentage is not valid.", nameof(value));

        Value = value;

    }
}