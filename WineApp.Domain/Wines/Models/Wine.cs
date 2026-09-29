using WineApp.Domain.Wines.Enums;

namespace WineApp.Domain.Wines.Models;

public class Wine
{
    public Guid WineId { get; set; }
    public string WineName { get; set; } = string.Empty;
    public string WineDescription { get; set; } = string.Empty;
    public string WineProducer { get; set; } = string.Empty;
    public string WineCountry { get; set; } = string.Empty;
    public string? WineRegion { get; set; }
    public WineType WineType { get; set; }
    public string? CustomWineType {  get; set; }
    public string WineGrape { get; set; } = string.Empty;
    public int WineYear { get; set; }
    public decimal AlcoholPercentage { get; set; }
    public string? RecommendedFood { get; set; }
}
