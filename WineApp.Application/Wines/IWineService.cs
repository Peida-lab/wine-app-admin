using WineApp.Domain.Wines.Enums;

namespace WineApp.Application.Wines
{
    public interface IWineService
    {
        Task AddWineAsync(string wineName, string wineDescription, string wineProducer, string wineCountry, string? wineRegion, WineType wineType, string? customWineType, string wineGrape, int wineYear, decimal alcoholPercentage, string? recommendedFood);
    }
}