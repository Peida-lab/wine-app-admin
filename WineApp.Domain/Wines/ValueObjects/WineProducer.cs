namespace WineApp.Domain.Wines.ValueObjects;

public record WineProducer
{
    public string Value { get; }
    public WineProducer(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wine producer is required.", nameof(value));
        value = value.Trim();
        Value = value;
    }
}