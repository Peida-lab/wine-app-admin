namespace WineApp.Domain.Wines.ValueObjects;

public record WineCountry
{
    public string Value { get; }
    public WineCountry(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wine country is required.", nameof(value));
        value = value.Trim();
        Value = value;
    }
}
