using WineApp.Application.Wines.Interfaces;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;
using WineApp.Domain.Wines.ValueObjects;

namespace WineApp.Application.Wines;

public class WineService : IWineService
{
    private readonly IWineRepository _wineRepository;
    public WineService(IWineRepository wineRepository)
    {
        _wineRepository = wineRepository;
    }
    public async Task AddWineAsync(
        string wineName,
        string wineDescription,
        string wineProducer,
        string wineCountry,
        string? wineRegion,
        WineType wineType,
        string? customWineType,
        string wineGrape,
        int wineYear,
        decimal alcoholPercentage,
        string? recommendedFood)
    {
        WineName validatedWineName = new(wineName);
        WineDescription validatedDescription = new(wineDescription);
        WineProducer validatedProducer = new(wineProducer);
        WineCountry validatedCountry = new(wineCountry);
        WineGrape validatedGrape = new(wineGrape);
        WineYear validatedYear = new(wineYear);
        WinePercentage validatedPercentage = new(alcoholPercentage);

        if (wineType == WineType.Other && string.IsNullOrWhiteSpace(customWineType))
            throw new ArgumentException("Wine type is required when type is Other.",
                nameof(customWineType));
        if (wineType != WineType.Other)
            customWineType = null;
        else
            customWineType = customWineType!.Trim();

        Wine wine = new()
        {
            WineId = Guid.NewGuid(),
            WineName = validatedWineName,
            WineDescription = validatedDescription,
            WineProducer = validatedProducer,
            WineCountry = validatedCountry,
            WineRegion = wineRegion,
            WineType = wineType,
            CustomWineType = customWineType,
            WineGrape = validatedGrape,
            WineYear = validatedYear,
            AlcoholPercentage = validatedPercentage,
            RecommendedFood = recommendedFood

        };

        await _wineRepository.AddWineAsync(wine);



    }
}
