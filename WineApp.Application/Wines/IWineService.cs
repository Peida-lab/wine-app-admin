using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;

namespace WineApp.Application.Wines
{
    public interface IWineService
    {
        Task AddWineAsync(string wineName, string wineDescription, string wineProducer, string wineCountry, string? wineRegion, WineType wineType, string? customWineType, string wineGrape, int wineYear, decimal alcoholPercentage, string? recommendedFood);
        Task DeleteWineAsync(Guid wineId);
        Task<List<Wine>> GetAllWinesAsync();
        Task<Wine?> GetWineByIdAsync(Guid wineId);
        Task UpdateWineAsync(Wine wine);
    }
}