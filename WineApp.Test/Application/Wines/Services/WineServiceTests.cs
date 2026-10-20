
using WineApp.Application.Images;
using WineApp.Application.Wines;
using WineApp.Application.Wines.Interfaces;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;

namespace WineApp.Test.Application.Wines.Services;

public class WineServiceTests
{
    private readonly FakeWineRepository _repository = new();
    private readonly FakeImageStorageService _imageStorage = new();
    private readonly WineService _service;

    public WineServiceTests()
    {
        _service = new WineService(_repository, _imageStorage);
    }

    // Helper for registering a valid wine
    private Task AddValidWineAsync(
        Stream? imageStream = null,
        string? imageFileName = null,
        WineType wineType = WineType.Red,
        string? customWineType = null,
        string wineName = "Test Wine")
    {
        return _service.AddWineAsync(
            wineName: wineName,
            wineDescription: "A test wine description",
            wineProducer: "Test Producer",
            wineCountry: "France",
            wineRegion: "Bordeaux",
            wineType: wineType,
            customWineType: customWineType,
            wineGrape: "Merlot",
            wineYear: 2020,
            alcoholPercentage: 13.5m,
            recommendedFood: "Pasta",
            imageStream: imageStream,
            imageFileName: imageFileName);
    }

    [Fact]
    public async Task AddWineAsync_WithoutImage_SavesWine()
    {
        // Act
        await AddValidWineAsync();

        // Assert
        Wine wine = Assert.Single(_repository.Wines);

        Assert.NotEqual(Guid.Empty, wine.WineId);
        Assert.Equal("Test Wine", wine.WineName.Value);
        Assert.Null(wine.ImageReference);
        Assert.Equal(0, _imageStorage.SaveCallCount);
    }

    [Fact]
    public async Task AddWineAsync_WithImage_SavesImageReference()
    {
        // Arrange
        using MemoryStream imageStream = new([1, 2, 3, 4]);

        _imageStorage.ReferenceToReturn = "unique-image.jpg";

        // Act
        await AddValidWineAsync(
            imageStream: imageStream,
            imageFileName: "wine.jpg");

        // Assert
        Wine wine = Assert.Single(_repository.Wines);

        Assert.Equal("unique-image.jpg", wine.ImageReference);
        Assert.Equal(1, _imageStorage.SaveCallCount);
        Assert.Equal("wine.jpg", _imageStorage.LastFileName);
        Assert.Same(imageStream, _imageStorage.LastStream);
    }

    [Fact]
    public async Task AddWineAsync_WithImageButNoFileName_ThrowsException()
    {
        // Arrange
        using MemoryStream imageStream = new([1, 2, 3]);

        // Act
        Func<Task> action = () =>
            AddValidWineAsync(imageStream: imageStream);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        Assert.Empty(_repository.Wines);
        Assert.Equal(0, _imageStorage.SaveCallCount);
    }

    [Fact]
    public async Task AddWineAsync_WithFileNameButNoImage_ThrowsException()
    {
        // Act
        Func<Task> action = () =>
            AddValidWineAsync(imageFileName: "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        Assert.Empty(_repository.Wines);
        Assert.Equal(0, _imageStorage.SaveCallCount);
    }

    [Fact]
    public async Task AddWineAsync_WhenWineNameIsInvalid_ThrowsException()
    {
        // Arrange
        using MemoryStream imageStream = new([1, 2, 3]);

        // Act
        Func<Task> action = () =>
            AddValidWineAsync(
                imageStream: imageStream,
                imageFileName: "wine.jpg",
                wineName: "");

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        Assert.Empty(_repository.Wines);
        Assert.Equal(0, _imageStorage.SaveCallCount);
    }

    [Fact]
    public async Task AddWineAsync_WhenImageStorageFails_DoesNotSaveWine()
    {
        // Arrange
        using MemoryStream imageStream = new([1, 2, 3]);

        _imageStorage.ShouldFail = true;

        // Act
        Func<Task> action = () =>
            AddValidWineAsync(
                imageStream: imageStream,
                imageFileName: "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<IOException>(action);

        Assert.Empty(_repository.Wines);
        Assert.Equal(1, _imageStorage.SaveCallCount);
    }

    [Fact]
    public async Task AddWineAsync_WhenTypeIsOtherWithoutCustomType_ThrowsException()
    {
        // Act
        Func<Task> action = () =>
            AddValidWineAsync(
                wineType: WineType.Other,
                customWineType: null);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        Assert.Empty(_repository.Wines);
    }

    [Fact]
    public async Task AddWineAsync_WhenTypeIsOther_TrimsCustomType()
    {
        // Act
        await AddValidWineAsync(
            wineType: WineType.Other,
            customWineType: "  Orange Wine  ");

        // Assert
        Wine wine = Assert.Single(_repository.Wines);

        Assert.Equal(WineType.Other, wine.WineType);
        Assert.Equal("Orange Wine", wine.CustomWineType);
    }

    [Fact]
    public async Task AddWineAsync_WhenTypeIsNotOther_ClearsCustomType()
    {
        // Act
        await AddValidWineAsync(
            wineType: WineType.Red,
            customWineType: "Orange Wine");

        // Assert
        Wine wine = Assert.Single(_repository.Wines);

        Assert.Equal(WineType.Red, wine.WineType);
        Assert.Null(wine.CustomWineType);
    }

    // Fake repository used for testing
    private class FakeWineRepository : IWineRepository
    {
        public List<Wine> Wines { get; } = [];

        public Task AddWineAsync(Wine wine)
        {
            Wines.Add(wine);
            return Task.CompletedTask;
        }

        public Task<List<Wine>> GetAllWinesAsync()
        {
            return Task.FromResult(Wines.ToList());
        }

        public Task<Wine?> GetWineByIdAsync(Guid wineId)
        {
            Wine? wine = Wines.FirstOrDefault(
                w => w.WineId == wineId);

            return Task.FromResult(wine);
        }

        public Task UpdateWineAsync(Wine wine)
        {
            int index = Wines.FindIndex(
                w => w.WineId == wine.WineId);

            if (index < 0)
                throw new InvalidOperationException("Wine not found.");

            Wines[index] = wine;

            return Task.CompletedTask;
        }

        public Task DeleteWineAsync(Guid wineId)
        {
            Wines.RemoveAll(w => w.WineId == wineId);
            return Task.CompletedTask;
        }
    }

    // Fake image storage used for testing
    private class FakeImageStorageService : IImageStorageService
    {
        public int SaveCallCount { get; private set; }

        public string ReferenceToReturn { get; set; } =
            "test-image.jpg";

        public string? LastFileName { get; private set; }

        public Stream? LastStream { get; private set; }

        public bool ShouldFail { get; set; }

        public Task<string> SaveImageAsync(
            Stream imageStream,
            string fileName)
        {
            SaveCallCount++;

            LastStream = imageStream;
            LastFileName = fileName;

            if (ShouldFail)
                throw new IOException("Failed to save image.");

            return Task.FromResult(ReferenceToReturn);
        }
    }
}
