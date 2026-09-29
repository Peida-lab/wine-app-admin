namespace WineApp.Domain.Wines.ValueObjects;

public record WineDescription
{
    public string Value { get;}
    public WineDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wine description is required.", nameof(value));
        value = value.Trim();

        if (value.Length > 1000)
            throw new ArgumentException("Wine description is too long.", nameof(value));

        Value = value;
    }
}
