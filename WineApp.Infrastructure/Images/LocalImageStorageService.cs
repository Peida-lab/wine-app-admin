
using WineApp.Application.Images;

namespace WineApp.Infrastructure.Images;

public class LocalImageStorageService : IImageStorageService
{
    private const long MaxImageSize = 10 * 1024 * 1024;
    private readonly string _folderPath;

    public LocalImageStorageService(string folderPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        _folderPath = folderPath;
        Directory.CreateDirectory(_folderPath);
    }

    public async Task<string> SaveImageAsync(
        Stream imageStream,
        string fileName)
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        if (!imageStream.CanRead)
            throw new ArgumentException(
                "Image stream must be readable.",
                nameof(imageStream));

        string extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (extension is not (".jpg" or ".jpeg" or ".png"))
            throw new ArgumentException(
                "Only JPG and PNG images are supported.",
                nameof(fileName));

        // Read the file signature to verify its format.
        byte[] header = new byte[8];
        int headerLength = 0;

        while (headerLength < header.Length)
        {
            int read = await imageStream.ReadAsync(
                header.AsMemory(headerLength));

            if (read == 0)
                break;

            headerLength += read;
        }

        bool isJpeg = headerLength >= 3 &&
                      header[0] == 0xFF &&
                      header[1] == 0xD8 &&
                      header[2] == 0xFF;

        bool isPng = headerLength == 8 &&
                     header.SequenceEqual(
                         new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        if (extension is ".jpg" or ".jpeg" ? !isJpeg : !isPng)
            throw new InvalidDataException(
                "The file content does not match its image format.");

        // Generate a unique filename.
        string uniqueFileName = $"{Guid.NewGuid():N}{extension}";
        string fullPath = Path.Combine(_folderPath, uniqueFileName);

        try
        {
            await using FileStream fileStream = new(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                useAsync: true);

            await fileStream.WriteAsync(
                header.AsMemory(0, headerLength));

            byte[] buffer = new byte[81920];
            long totalBytes = headerLength;

            int bytesRead;

            while ((bytesRead = await imageStream.ReadAsync(buffer)) > 0)
            {
                totalBytes += bytesRead;

                if (totalBytes > MaxImageSize)
                    throw new InvalidDataException(
                        "Image cannot exceed 10 MB.");

                await fileStream.WriteAsync(
                    buffer.AsMemory(0, bytesRead));
            }
        }
        catch
        {
            // Remove incomplete files if saving fails.
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            throw;
        }

        return uniqueFileName;
    }
}
