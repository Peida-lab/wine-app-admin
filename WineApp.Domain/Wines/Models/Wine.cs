using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.ValueObjects;

namespace WineApp.Domain.Wines.Models;

public class Wine
{
    public Guid WineId { get; set; }
    public required WineName WineName { get; set; }
    public string WineDescription { get; set; } = string.Empty;
    public required WineProducer WineProducer { get; set; }
    public required WineCountry WineCountry { get; set; }
    public string? WineRegion { get; set; }
    public WineType WineType { get; set; }
    public string? CustomWineType { get; set; }
    public string WineGrape { get; set; } = string.Empty;
    public int WineYear { get; set; }
    public decimal AlcoholPercentage { get; set; }
    public string? RecommendedFood { get; set; }
}
