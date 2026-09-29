using System.Text.Json;
using WineApp.Application.Wines.Interfaces;
using WineApp.Domain.Wines.Models;
using WineApp.Infrastructure.Wines.Models;

namespace WineApp.Infrastructure.Wines;

public class JsonWineRepository : IWineRepository
{
    private readonly string _filePath;
    public JsonWineRepository(string filePath) => _filePath = filePath;

    public async Task AddWineAsync(Wine wine)
    {
        List<WineData> wines = [];
        if (File.Exists(_filePath))
        {
            string json = await File.ReadAllTextAsync(_filePath);
            wines = JsonSerializer.Deserialize<List<WineData>>(json) ?? [];
        }
        WineData wineData = new()
        {
            WineId = wine.WineId,
            WineName = wine.WineName.Value,
            WineDescription = wine.WineDescription.Value,
            WineProducer = wine.WineProducer.Value,
            WineCountry = wine.WineCountry.Value,
            WineRegion = wine.WineRegion,
            WineGrape = wine.WineGrape.Value,
            WineType = wine.WineType,
            WineYear = wine.WineYear.Value,
            CustomWineType = wine.CustomWineType,
            AlcoholPercentage = wine.AlcoholPercentage.Value,
            RecommendedFood = wine.RecommendedFood
        };
        wines.Add(wineData);

        string updatedJson = JsonSerializer.Serialize(wines);

        await File.WriteAllTextAsync(_filePath, updatedJson);

    }

    public Task DeleteWineAsync(Guid wineId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Wine>> GetAllWinesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Wine?> GetWineByIdAsync(Guid wineId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateWineAsync(Wine wine)
    {
        throw new NotImplementedException();
    }
}
