namespace WineApp.Domain.Wines.ValueObjects;

public record WineGrape
{
    public string Value { get; }
    public WineGrape(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wine grape is required.", nameof(value));
        value = value.Trim();
        Value = value;
    }
}
