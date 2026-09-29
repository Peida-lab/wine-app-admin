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
    public required WineGrape WineGrape { get; set; }
    public required WineYear WineYear { get; set; }
    public required WinePercentage AlcoholPercentage { get; set; }
    public string? RecommendedFood { get; set; }
}
