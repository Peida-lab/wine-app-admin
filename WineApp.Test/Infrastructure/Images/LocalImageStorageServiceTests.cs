
using WineApp.Infrastructure.Images;

namespace WineApp.Test.Infrastructure.Images;

public class LocalImageStorageServiceTests : IDisposable
{
    private readonly string _folderPath;
    private readonly LocalImageStorageService _service;

    public LocalImageStorageServiceTests()
    {
        _folderPath = Path.Combine(
            Path.GetTempPath(),
            "WineAppTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_folderPath);

        _service = new LocalImageStorageService(_folderPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_folderPath))
            Directory.Delete(_folderPath, true);
    }

    private static byte[] CreateJpegBytes()
    {
        return [0xFF, 0xD8, 0xFF, 0xE0, 0x01, 0x02, 0x03, 0x04];
    }

    private static byte[] CreatePngBytes()
    {
        return [137, 80, 78, 71, 13, 10, 26, 10, 1, 2, 3, 4];
    }

    [Fact]
    public async Task SaveImageAsync_WhenJpegIsValid_SavesImage()
    {
        // Arrange
        byte[] imageBytes = CreateJpegBytes();
        using MemoryStream stream = new(imageBytes);

        // Act
        string fileName = await _service.SaveImageAsync(
            stream, "wine.jpg");

        string fullPath = Path.Combine(_folderPath, fileName);

        // Assert
        Assert.True(File.Exists(fullPath));
        Assert.EndsWith(".jpg", fileName);

        byte[] savedBytes = await File.ReadAllBytesAsync(fullPath);
        Assert.Equal(imageBytes, savedBytes);
    }

    [Fact]
    public async Task SaveImageAsync_WhenPngIsValid_SavesImage()
    {
        // Arrange
        byte[] imageBytes = CreatePngBytes();
        using MemoryStream stream = new(imageBytes);

        // Act
        string fileName = await _service.SaveImageAsync(
            stream, "wine.png");

        // Assert
        Assert.EndsWith(".png", fileName);
        Assert.True(File.Exists(
            Path.Combine(_folderPath, fileName)));
    }

    [Fact]
    public async Task SaveImageAsync_WhenCalledTwice_GeneratesUniqueNames()
    {
        // Arrange
        using MemoryStream firstStream = new(CreateJpegBytes());
        using MemoryStream secondStream = new(CreateJpegBytes());

        // Act
        string firstName = await _service.SaveImageAsync(
            firstStream, "wine.jpg");

        string secondName = await _service.SaveImageAsync(
            secondStream, "wine.jpg");

        // Assert
        Assert.NotEqual(firstName, secondName);
        Assert.Equal(2, Directory.GetFiles(_folderPath).Length);
    }

    [Theory]
    [InlineData("wine.gif")]
    [InlineData("wine.bmp")]
    [InlineData("wine.txt")]
    public async Task SaveImageAsync_WhenExtensionIsInvalid_ThrowsException(
        string fileName)
    {
        // Arrange
        using MemoryStream stream = new(CreateJpegBytes());

        // Act
        Func<Task> action = () =>
            _service.SaveImageAsync(stream, fileName);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
        Assert.Empty(Directory.GetFiles(_folderPath));
    }

    [Fact]
    public async Task SaveImageAsync_WhenContentDoesNotMatchExtension_ThrowsException()
    {
        // Arrange
        using MemoryStream stream = new(CreatePngBytes());

        // Act
        Func<Task> action = () =>
            _service.SaveImageAsync(stream, "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<InvalidDataException>(action);
        Assert.Empty(Directory.GetFiles(_folderPath));
    }

    [Fact]
    public async Task SaveImageAsync_WhenImageIsTooLarge_ThrowsException()
    {
        // Arrange
        byte[] imageBytes = new byte[10 * 1024 * 1024 + 1];

        imageBytes[0] = 0xFF;
        imageBytes[1] = 0xD8;
        imageBytes[2] = 0xFF;

        using MemoryStream stream = new(imageBytes);

        // Act
        Func<Task> action = () =>
            _service.SaveImageAsync(stream, "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<InvalidDataException>(action);

        Assert.Empty(Directory.GetFiles(_folderPath));
    }

    [Fact]
    public async Task SaveImageAsync_WhenStreamIsEmpty_ThrowsException()
    {
        // Arrange
        using MemoryStream stream = new();

        // Act
        Func<Task> action = () =>
            _service.SaveImageAsync(stream, "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<InvalidDataException>(action);
    }

    [Fact]
    public async Task SaveImageAsync_WhenFileNameIsEmpty_ThrowsException()
    {
        // Arrange
        using MemoryStream stream = new(CreateJpegBytes());

        // Act
        Func<Task> action = () =>
            _service.SaveImageAsync(stream, "");

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(action);
    }

    [Fact]
    public async Task SaveImageAsync_WhenStreamIsNull_ThrowsException()
    {
        // Act
        Func<Task> action = () =>
            _service.SaveImageAsync(null!, "wine.jpg");

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(action);
    }

    [Fact]
    public async Task DeleteImageAsync_WhenImageExists_DeletesFile()
    {
        // Arrange
        using MemoryStream stream = new(CreateJpegBytes());

        string imageReference =
            await _service.SaveImageAsync(stream, "wine.jpg");

        string imagePath =
            Path.Combine(_folderPath, imageReference);

        Assert.True(File.Exists(imagePath));

        // Act
        await _service.DeleteImageAsync(imageReference);

        // Assert
        Assert.False(File.Exists(imagePath));
    }

}
