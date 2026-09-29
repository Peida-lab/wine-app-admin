using System.Text.Json;
using WineApp.Application.Wines.Interfaces;
using WineApp.Domain.Wines.Models;
using WineApp.Domain.Wines.ValueObjects;
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

    public async Task DeleteWineAsync(Guid wineId)
    {
        if (!File.Exists(_filePath))
            throw new InvalidOperationException("Wine file does not exist.");

        string json = await File.ReadAllTextAsync(_filePath);

        List<WineData> wines =
            JsonSerializer.Deserialize<List<WineData>>(json) ?? [];

        WineData? wineToDelete =
            wines.FirstOrDefault(w => w.WineId == wineId);

        if (wineToDelete == null)
            throw new InvalidOperationException("Wine not found.");

        wines.Remove(wineToDelete);

        string updatedJson = JsonSerializer.Serialize(wines);

        await File.WriteAllTextAsync(_filePath, updatedJson);
    }

    public async Task<List<Wine>> GetAllWinesAsync()
    {
        if (!File.Exists(_filePath))
            return [];
        string json = await File.ReadAllTextAsync(_filePath);

        List<WineData> wineDataList =
            JsonSerializer.Deserialize<List<WineData>>(json) ?? [];

        List<Wine> wines = [];
        foreach (WineData wineData in wineDataList)
        {
            Wine wine = new()
            {
                WineId = wineData.WineId,
                WineName = new WineName(wineData.WineName),
                WineDescription = new WineDescription(wineData.WineDescription),
                WineProducer = new WineProducer(wineData.WineProducer),
                WineCountry = new WineCountry(wineData.WineCountry),
                WineRegion = wineData.WineRegion,
                WineType = wineData.WineType,
                CustomWineType = wineData.CustomWineType,
                WineGrape = new WineGrape(wineData.WineGrape),
                WineYear = new WineYear(wineData.WineYear),
                AlcoholPercentage = new WinePercentage(wineData.AlcoholPercentage),
                RecommendedFood = wineData.RecommendedFood

            };

            wines.Add(wine);
        }
        return wines;
    }

    public async Task<Wine?> GetWineByIdAsync(Guid wineId)
    {
        if(!File.Exists(_filePath)) return null;

        string json = await File.ReadAllTextAsync(_filePath);

        List<WineData> wineDataList =
            JsonSerializer.Deserialize<List<WineData>>(json) ?? [];

        WineData? wineData = wineDataList.FirstOrDefault(w => w.WineId == wineId);

        if(wineData == null) return null;

        Wine wine = new()
        {
            WineId = wineData.WineId,
            WineName = new WineName(wineData.WineName),
            WineDescription = new WineDescription(wineData.WineDescription),
            WineProducer = new WineProducer(wineData.WineProducer),
            WineCountry = new WineCountry(wineData.WineCountry),
            WineRegion = wineData.WineRegion,
            WineType = wineData.WineType,
            CustomWineType = wineData.CustomWineType,
            WineGrape = new WineGrape(wineData.WineGrape),
            WineYear = new WineYear(wineData.WineYear),
            AlcoholPercentage = new WinePercentage(wineData.AlcoholPercentage),
            RecommendedFood = wineData.RecommendedFood
        };

        return wine;
    }

    public async Task UpdateWineAsync(Wine wine)
    {
        if (!File.Exists(_filePath))
            throw new InvalidOperationException("Wine file does not exist.");

        string json = await File.ReadAllTextAsync(_filePath);

        List<WineData> wines =
            JsonSerializer.Deserialize<List<WineData>>(json) ?? [];

        WineData? existingWine =
            wines.FirstOrDefault(w => w.WineId == wine.WineId);

        if (existingWine == null)
            throw new InvalidOperationException("Wine not found.");

        existingWine.WineName = wine.WineName.Value;
        existingWine.WineDescription = wine.WineDescription.Value;
        existingWine.WineProducer = wine.WineProducer.Value;
        existingWine.WineCountry = wine.WineCountry.Value;
        existingWine.WineRegion = wine.WineRegion;
        existingWine.WineType = wine.WineType;
        existingWine.CustomWineType = wine.CustomWineType;
        existingWine.WineGrape = wine.WineGrape.Value;
        existingWine.WineYear = wine.WineYear.Value;
        existingWine.AlcoholPercentage = wine.AlcoholPercentage.Value;
        existingWine.RecommendedFood = wine.RecommendedFood;

        string updatedJson = JsonSerializer.Serialize(wines);

        await File.WriteAllTextAsync(_filePath, updatedJson);
    }
}
