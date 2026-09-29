namespace WineApp.Domain.Wines.ValueObjects;

public record WineName
{
    public string Value { get; }
    public WineName(string value) 
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wine name is required.", nameof(value));
        value = value.Trim();
        Value = value; 
    }
}
