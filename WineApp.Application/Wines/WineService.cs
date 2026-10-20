
using WineApp.Application.Images;
using WineApp.Application.Wines.Interfaces;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;
using WineApp.Domain.Wines.ValueObjects;

namespace WineApp.Application.Wines;

public class WineService : IWineService
{
    private readonly IWineRepository _wineRepository;
    private readonly IImageStorageService _imageStorageService;

    public WineService(
        IWineRepository wineRepository,
        IImageStorageService imageStorageService)
    {
        _wineRepository = wineRepository;
        _imageStorageService = imageStorageService;
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
        string? recommendedFood,
        Stream? imageStream = null,
        string? imageFileName = null,
        bool isFineWine = false)
    {
        // Validate wine information
        WineName validatedWineName = new(wineName);
        WineDescription validatedDescription = new(wineDescription);
        WineProducer validatedProducer = new(wineProducer);
        WineCountry validatedCountry = new(wineCountry);
        WineGrape validatedGrape = new(wineGrape);
        WineYear validatedYear = new(wineYear);
        WinePercentage validatedPercentage = new(alcoholPercentage);

        if (wineType == WineType.Other &&
            string.IsNullOrWhiteSpace(customWineType))
        {
            throw new ArgumentException(
                "Wine type is required when type is Other.",
                nameof(customWineType));
        }

        if (wineType != WineType.Other)
            customWineType = null;
        else
            customWineType = customWineType!.Trim();

        // Validate optional image
        if (imageStream is not null &&
            string.IsNullOrWhiteSpace(imageFileName))
        {
            throw new ArgumentException(
                "Image filename is required when uploading an image.",
                nameof(imageFileName));
        }

        if (imageStream is null && imageFileName is not null)
        {
            throw new ArgumentException(
                "Image stream is required when providing a filename.",
                nameof(imageStream));
        }

        // Save image if one was provided
        string? imageReference = null;

        if (imageStream is not null)
        {
            imageReference =
                await _imageStorageService.SaveImageAsync(
                    imageStream,
                    imageFileName!);
        }

        // Create wine
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
            IsFineWine = isFineWine,
            WineGrape = validatedGrape,
            WineYear = validatedYear,
            AlcoholPercentage = validatedPercentage,
            RecommendedFood = recommendedFood,
            ImageReference = imageReference
        };

        // Save wine and clean up image if saving fails
        try
        {
            await _wineRepository.AddWineAsync(wine);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(imageReference))
            {
                await TryDeleteImageAsync(imageReference);
            }

            throw;
        }
    }

    public async Task<List<Wine>> GetAllWinesAsync()
    {
        return await _wineRepository.GetAllWinesAsync();
    }

    public async Task<Wine?> GetWineByIdAsync(Guid wineId)
    {
        return await _wineRepository.GetWineByIdAsync(wineId);
    }

    public async Task DeleteWineAsync(Guid wineId)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException(
                "Wine id is required.",
                nameof(wineId));
        }

        // Get the wine before deleting it
        Wine? wine =
            await _wineRepository.GetWineByIdAsync(wineId);

        // Delete wine from repository first
        await _wineRepository.DeleteWineAsync(wineId);

        // Delete associated image after successful deletion
        if (!string.IsNullOrWhiteSpace(wine?.ImageReference))
        {
            await TryDeleteImageAsync(wine.ImageReference);
        }
    }

    public async Task UpdateWineAsync(
        Wine wine,
        Stream? imageStream = null,
        string? imageFileName = null)
    {
        ArgumentNullException.ThrowIfNull(wine);

        if (wine.WineId == Guid.Empty)
        {
            throw new ArgumentException(
                "Wine id is required.");
        }

        if (wine.WineType == WineType.Other &&
            string.IsNullOrWhiteSpace(wine.CustomWineType))
        {
            throw new ArgumentException(
                "Custom wine type is required when type is Other.");
        }

        if (wine.WineType != WineType.Other)
            wine.CustomWineType = null;
        else
            wine.CustomWineType = wine.CustomWineType!.Trim();

        // Validate optional image
        if (imageStream is not null &&
            string.IsNullOrWhiteSpace(imageFileName))
        {
            throw new ArgumentException(
                "Image filename is required.");
        }

        if (imageStream is null && imageFileName is not null)
        {
            throw new ArgumentException(
                "Image stream is required.");
        }

        // Get previously saved wine
        Wine? existingWine =
            await _wineRepository.GetWineByIdAsync(wine.WineId);

        if (existingWine is null)
        {
            throw new InvalidOperationException(
                "Wine not found.");
        }

        string? oldImageReference =
            existingWine.ImageReference;

        // No new image: update wine information only
        if (imageStream is null)
        {
            await _wineRepository.UpdateWineAsync(wine);
            return;
        }

        // Save replacement image
        string newImageReference =
            await _imageStorageService.SaveImageAsync(
                imageStream,
                imageFileName!);

        wine.ImageReference = newImageReference;

        try
        {
            // Update JSON before deleting old image
            await _wineRepository.UpdateWineAsync(wine);
        }
        catch
        {
            // Restore previous image reference
            wine.ImageReference = oldImageReference;

            // Remove newly uploaded image
            await TryDeleteImageAsync(newImageReference);

            throw;
        }

        // Delete old image after successful update
        if (!string.IsNullOrWhiteSpace(oldImageReference) &&
            oldImageReference != newImageReference)
        {
            await TryDeleteImageAsync(oldImageReference);
        }
    }

    // Remove an image without deleting the wine
    public async Task RemoveWineImageAsync(Guid wineId)
    {
        if (wineId == Guid.Empty)
        {
            throw new ArgumentException(
                "Wine id is required.",
                nameof(wineId));
        }

        // Retrieve the existing wine
        Wine? wine =
            await _wineRepository.GetWineByIdAsync(wineId);

        if (wine is null)
        {
            throw new InvalidOperationException(
                "Wine not found.");
        }

        // Nothing to remove
        if (string.IsNullOrWhiteSpace(wine.ImageReference))
            return;

        // Remember the old image
        string oldImageReference = wine.ImageReference;

        // Remove reference from wine
        wine.ImageReference = null;

        try
        {
            // Update JSON before deleting the image file
            await _wineRepository.UpdateWineAsync(wine);
        }
        catch
        {
            // Restore reference if repository update fails
            wine.ImageReference = oldImageReference;
            throw;
        }

        // Delete image after successful repository update
        await TryDeleteImageAsync(oldImageReference);
    }

    // Best-effort cleanup of stored image files
    private async Task TryDeleteImageAsync(string imageReference)
    {
        try
        {
            await _imageStorageService.DeleteImageAsync(
                imageReference);
        }
        catch (IOException)
        {
            // Image cleanup can be retried later
        }
        catch (UnauthorizedAccessException)
        {
            // Keep the original operation's result
        }
        catch (ArgumentException)
        {
            // Ignore invalid legacy image references
        }
    }
}
