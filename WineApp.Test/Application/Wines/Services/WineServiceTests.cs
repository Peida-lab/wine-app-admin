
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
        string wineName = "Test Wine",
        bool isFineWine = false)
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
            imageFileName: imageFileName,
            isFineWine: isFineWine);
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
        Assert.False(wine.IsFineWine);
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

    // New test: clean up uploaded image when repository fails
    [Fact]
    public async Task AddWineAsync_WhenRepositoryFails_DeletesUploadedImage()
    {
        // Arrange
        using MemoryStream imageStream = new([1, 2, 3, 4]);

        _imageStorage.ReferenceToReturn = "new-image.jpg";
        _repository.ShouldFailOnAdd = true;

        // Act
        Func<Task> action = () =>
            AddValidWineAsync(
                imageStream: imageStream,
                imageFileName: "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<IOException>(action);

        Assert.Empty(_repository.Wines);
        Assert.Equal(1, _imageStorage.SaveCallCount);

        // The uploaded image must be deleted
        Assert.Equal(1, _imageStorage.DeleteCallCount);

        Assert.Equal(
            "new-image.jpg",
            _imageStorage.LastDeletedImageReference);
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

    [Fact]
    public async Task AddWineAsync_WhenFineWine_SavesFineWineClassification()
    {
        await AddValidWineAsync(isFineWine: true);

        Wine wine = Assert.Single(_repository.Wines);

        Assert.True(wine.IsFineWine);
    }

    [Fact]
    public async Task UpdateWineAsync_WhenFineWineClassificationChanges_UpdatesWine()
    {
        await AddValidWineAsync();

        Wine wine = Assert.Single(_repository.Wines);

        wine.IsFineWine = true;

        await _service.UpdateWineAsync(wine);

        Assert.True(Assert.Single(_repository.Wines).IsFineWine);
    }

    [Fact]
    public async Task DeleteWineAsync_WhenWineHasImage_DeletesWineAndImage()
    {
        // Arrange
        using MemoryStream imageStream = new([1, 2, 3, 4]);

        await AddValidWineAsync(
            imageStream: imageStream,
            imageFileName: "wine.jpg");

        Wine wine = Assert.Single(_repository.Wines);

        // Act
        await _service.DeleteWineAsync(wine.WineId);

        // Assert
        Assert.Empty(_repository.Wines);
        Assert.Equal(1, _imageStorage.DeleteCallCount);

        Assert.Equal(
            "test-image.jpg",
            _imageStorage.LastDeletedImageReference);
    }

    [Fact]
    public async Task UpdateWineAsync_WhenImageIsReplaced_DeletesOldImage()
    {
        // Arrange
        using MemoryStream oldImageStream = new([1, 2, 3]);

        _imageStorage.ReferenceToReturn = "old-image.jpg";

        await AddValidWineAsync(
            imageStream: oldImageStream,
            imageFileName: "old.jpg");

        Wine wine = Assert.Single(_repository.Wines);

        using MemoryStream newImageStream = new([4, 5, 6]);

        _imageStorage.ReferenceToReturn = "new-image.jpg";

        // Act
        await _service.UpdateWineAsync(
            wine,
            imageStream: newImageStream,
            imageFileName: "new.jpg");

        // Assert
        Assert.Equal("new-image.jpg", wine.ImageReference);
        Assert.Equal(1, _imageStorage.DeleteCallCount);

        Assert.Equal(
            "old-image.jpg",
            _imageStorage.LastDeletedImageReference);
    }

    [Fact]
    public async Task UpdateWineAsync_WhenRepositoryFails_KeepsOldImage()
    {
        // Arrange
        using MemoryStream oldImageStream = new([1, 2, 3]);

        _imageStorage.ReferenceToReturn = "old-image.jpg";

        await AddValidWineAsync(
            imageStream: oldImageStream,
            imageFileName: "old.jpg");

        Wine wine = Assert.Single(_repository.Wines);

        _imageStorage.ReferenceToReturn = "new-image.jpg";
        _repository.ShouldFailOnUpdate = true;

        using MemoryStream newImageStream = new([4, 5, 6]);

        // Act
        Func<Task> action = () =>
            _service.UpdateWineAsync(
                wine,
                imageStream: newImageStream,
                imageFileName: "new.jpg");

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);

        Assert.Equal("old-image.jpg", wine.ImageReference);
        Assert.Equal(1, _imageStorage.DeleteCallCount);

        Assert.Equal(
            "new-image.jpg",
            _imageStorage.LastDeletedImageReference);
    }

    // Fake repository used for testing
    private class FakeWineRepository : IWineRepository
    {
        public List<Wine> Wines { get; } = [];

        public bool ShouldFailOnAdd { get; set; }
        public bool ShouldFailOnUpdate { get; set; }

        public Task AddWineAsync(Wine wine)
        {
            if (ShouldFailOnAdd)
            {
                throw new IOException(
                    "Simulated repository save failure.");
            }

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
            if (ShouldFailOnUpdate)
            {
                throw new InvalidOperationException(
                    "Simulated update failure.");
            }

            int index = Wines.FindIndex(
                w => w.WineId == wine.WineId);

            if (index < 0)
            {
                throw new InvalidOperationException(
                    "Wine not found.");
            }

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
        public int DeleteCallCount { get; private set; }

        public string ReferenceToReturn { get; set; } =
            "test-image.jpg";

        public string? LastFileName { get; private set; }
        public Stream? LastStream { get; private set; }
        public string? LastDeletedImageReference { get; private set; }

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

        public Task DeleteImageAsync(string imageReference)
        {
            DeleteCallCount++;
            LastDeletedImageReference = imageReference;

            return Task.CompletedTask;
        }
    }
}
