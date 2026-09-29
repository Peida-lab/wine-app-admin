namespace WineApp.Domain.Wines.ValueObjects;

public record WineYear
{
    public int Value { get; }
    public WineYear(int value)
    {
        if (value < 1800)
            throw new ArgumentException("Wine year is not valid.", nameof(value));
        if (value > DateTime.Now.Year)
            throw new ArgumentException("Wine year cannot be in the future", nameof(value));

        Value = value;
       
    }
}
