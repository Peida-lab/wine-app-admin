using System.Text.Json.Nodes;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;
using WineApp.Domain.Wines.ValueObjects;
using WineApp.Infrastructure.Wines;

namespace WineApp.Test.Infrastructure.Wines;

public class JsonWineRepositoryTests : IDisposable
{
    private readonly string _directory;
    private readonly string _filePath;
    private readonly JsonWineRepository _repository;

    public JsonWineRepositoryTests()
    {
        _directory = Path.Combine(
            Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directory);
        _filePath = Path.Combine(_directory, "wines.json");
        _repository = new JsonWineRepository(_filePath);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddWineAsync_PersistsFineWineClassification(bool isFineWine)
    {
        Wine wine = CreateWine(isFineWine);

        await _repository.AddWineAsync(wine);

        Wine? savedWine = await _repository.GetWineByIdAsync(wine.WineId);
        Assert.NotNull(savedWine);
        Assert.Equal(isFineWine, savedWine.IsFineWine);

        List<Wine> wines = await _repository.GetAllWinesAsync();
        Assert.Equal(isFineWine, Assert.Single(wines).IsFineWine);
    }

    [Fact]
    public async Task UpdateWineAsync_WhenFineWineChanges_PersistsUpdatedValue()
    {
        Wine wine = CreateWine(false);
        await _repository.AddWineAsync(wine);

        wine.IsFineWine = true;
        await _repository.UpdateWineAsync(wine);

        Wine? updatedWine = await _repository.GetWineByIdAsync(wine.WineId);
        Assert.NotNull(updatedWine);
        Assert.True(updatedWine.IsFineWine);

        wine.IsFineWine = false;
        await _repository.UpdateWineAsync(wine);

        updatedWine = await _repository.GetWineByIdAsync(wine.WineId);
        Assert.NotNull(updatedWine);
        Assert.False(updatedWine.IsFineWine);
    }

    [Fact]
    public async Task GetWine_WhenExistingJsonLacksFineWine_DefaultsToFalse()
    {
        Wine wine = CreateWine(true);
        await _repository.AddWineAsync(wine);

        JsonNode json = JsonNode.Parse(await File.ReadAllTextAsync(_filePath))!;
        JsonObject wineData = json.AsArray()[0]!.AsObject();
        Assert.True(wineData.Remove("IsFineWine"));
        await File.WriteAllTextAsync(_filePath, json.ToJsonString());

        Wine? loadedWine = await _repository.GetWineByIdAsync(wine.WineId);
        Assert.NotNull(loadedWine);
        Assert.False(loadedWine.IsFineWine);

        List<Wine> allWines = await _repository.GetAllWinesAsync();
        Assert.False(Assert.Single(allWines).IsFineWine);
    }

    private static Wine CreateWine(bool isFineWine) => new()
    {
        WineId = Guid.NewGuid(),
        WineName = new WineName("Test Wine"),
        WineDescription = new WineDescription("Test description"),
        WineProducer = new WineProducer("Test Producer"),
        WineCountry = new WineCountry("France"),
        WineRegion = "Bordeaux",
        WineType = WineType.Red,
        WineGrape = new WineGrape("Merlot"),
        WineYear = new WineYear(2020),
        AlcoholPercentage = new WinePercentage(13.5m),
        IsFineWine = isFineWine
    };

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }
}
