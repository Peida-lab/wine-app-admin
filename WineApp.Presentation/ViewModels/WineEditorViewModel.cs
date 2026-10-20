
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using WineApp.Application.Images;
using WineApp.Application.Wines;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;
using WineApp.Domain.Wines.ValueObjects;

namespace WineApp.Presentation.ViewModels;

public partial class WineEditorViewModel(
    IWineService wineService,
    IImageStorageService imageStorageService)
    : ObservableObject
{
    private readonly IWineService _wineService = wineService;
    private readonly IImageStorageService _imageStorageService = imageStorageService;

    private Wine? _editingWine;

    [ObservableProperty]
    public partial string WineName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineDescription { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineProducer { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineCountry { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineRegion { get; set; } = string.Empty;

    [ObservableProperty]
    public partial WineType SelectedWineType { get; set; } = WineType.Red;

    [ObservableProperty]
    public partial string CustomWineType { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineGrape { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineYear { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string AlcoholPercentage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string RecommendedFood { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsFineWine { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEditMode { get; set; }

    // Original image stored in the application
    [ObservableProperty]
    public partial string? CurrentImageReference { get; set; }

    // Newly selected image, before saving
    [ObservableProperty]
    public partial string? SelectedImagePath { get; set; }

    [ObservableProperty]
    public partial string SelectedImageName { get; set; } =
        "Ingen bild vald";

    public string PageTitle =>
        IsEditMode ? "Redigera vin" : "Lägg till vin";

    public string SaveButtonText =>
        IsEditMode ? "Spara ändringar" : "Spara vin";

    public IReadOnlyList<WineType> WineTypes { get; } =
        Enum.GetValues<WineType>();

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    // Remember the newly selected image
    public void SetSelectedImage(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        SelectedImagePath = filePath;
        SelectedImageName = Path.GetFileName(filePath);
    }

    public async Task LoadWineAsync(Guid? wineId)
    {
        _editingWine = null;
        IsEditMode = wineId.HasValue;
        ResetForm();
        StatusMessage = string.Empty;

        if (!wineId.HasValue)
            return;

        try
        {
            Wine? wine = await _wineService.GetWineByIdAsync(wineId.Value);

            if (wine is null)
            {
                StatusMessage = "Vinet kunde inte hittas.";
                return;
            }

            _editingWine = wine;

            WineName = wine.WineName.Value;
            WineDescription = wine.WineDescription.Value;
            WineProducer = wine.WineProducer.Value;
            WineCountry = wine.WineCountry.Value;
            WineRegion = wine.WineRegion ?? string.Empty;
            SelectedWineType = wine.WineType;
            CustomWineType = wine.CustomWineType ?? string.Empty;
            WineGrape = wine.WineGrape.Value;
            WineYear = wine.WineYear.Value.ToString();
            AlcoholPercentage = wine.AlcoholPercentage.Value.ToString(
                CultureInfo.GetCultureInfo("sv-SE"));
            RecommendedFood = wine.RecommendedFood ?? string.Empty;
            IsFineWine = wine.IsFineWine;

            // Display the existing saved image
            CurrentImageReference = wine.ImageReference;

            SelectedImageName =
                string.IsNullOrWhiteSpace(wine.ImageReference)
                    ? "Ingen bild vald"
                    : "Bild sparad – välj en ny bild för att ersätta";
        }
        catch (Exception)
        {
            StatusMessage = "Det gick inte att hämta vinet.";
        }
    }

    [RelayCommand]
    private async Task SaveWineAsync()
    {
        StatusMessage = string.Empty;

        if (IsEditMode && _editingWine is null)
        {
            StatusMessage = "Det finns inget vin att uppdatera.";
            return;
        }

        if (string.IsNullOrWhiteSpace(WineName) ||
            string.IsNullOrWhiteSpace(WineDescription) ||
            string.IsNullOrWhiteSpace(WineProducer) ||
            string.IsNullOrWhiteSpace(WineCountry) ||
            string.IsNullOrWhiteSpace(WineGrape))
        {
            StatusMessage = "Fyll i alla obligatoriska fält.";
            return;
        }

        if (!int.TryParse(WineYear, out int year))
        {
            StatusMessage = "Ange en giltig årgång.";
            return;
        }

        bool validPercentage = decimal.TryParse(
            AlcoholPercentage,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.GetCultureInfo("sv-SE"),
            out decimal percentage);

        if (!validPercentage)
        {
            validPercentage = decimal.TryParse(
                AlcoholPercentage,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out percentage);
        }

        if (!validPercentage)
        {
            StatusMessage = "Ange en giltig alkoholhalt.";
            return;
        }

        try
        {
            if (IsEditMode)
            {
                if (SelectedWineType == WineType.Other &&
                    string.IsNullOrWhiteSpace(CustomWineType))
                {
                    throw new ArgumentException(
                        "Ange en annan vintyp.");
                }

                // Validate values before updating
                WineName validatedName = new(WineName);
                WineDescription validatedDescription = new(WineDescription);
                WineProducer validatedProducer = new(WineProducer);
                WineCountry validatedCountry = new(WineCountry);
                WineGrape validatedGrape = new(WineGrape);
                WineYear validatedYear = new(year);
                WinePercentage validatedPercentage = new(percentage);

                Wine wine = _editingWine!;

                wine.WineName = validatedName;
                wine.WineDescription = validatedDescription;
                wine.WineProducer = validatedProducer;
                wine.WineCountry = validatedCountry;
                wine.WineRegion = WineRegion;
                wine.WineType = SelectedWineType;
                wine.CustomWineType = CustomWineType;
                wine.WineGrape = validatedGrape;
                wine.WineYear = validatedYear;
                wine.AlcoholPercentage = validatedPercentage;
                wine.RecommendedFood = RecommendedFood;
                wine.IsFineWine = IsFineWine;

                // Save replacement image, if selected
                if (SelectedImagePath is not null)
                {
                    using FileStream imageStream =
                        File.OpenRead(SelectedImagePath);

                    wine.ImageReference =
                        await _imageStorageService.SaveImageAsync(
                            imageStream,
                            Path.GetFileName(SelectedImagePath));
                }

                await _wineService.UpdateWineAsync(wine);

                // Switch preview to the saved image
                CurrentImageReference = wine.ImageReference;
                SelectedImagePath = null;

                SelectedImageName =
                    string.IsNullOrWhiteSpace(wine.ImageReference)
                        ? "Ingen bild vald"
                        : "Bild sparad – välj en ny bild för att ersätta";

                StatusMessage = "Ändringarna har sparats!";
            }
            else
            {
                using FileStream? imageStream =
                    SelectedImagePath is null
                        ? null
                        : File.OpenRead(SelectedImagePath);

                await _wineService.AddWineAsync(
                    wineName: WineName,
                    wineDescription: WineDescription,
                    wineProducer: WineProducer,
                    wineCountry: WineCountry,
                    wineRegion: WineRegion,
                    wineType: SelectedWineType,
                    customWineType: CustomWineType,
                    wineGrape: WineGrape,
                    wineYear: year,
                    alcoholPercentage: percentage,
                    recommendedFood: RecommendedFood,
                    imageStream: imageStream,
                    imageFileName: SelectedImagePath is null
                        ? null
                        : Path.GetFileName(SelectedImagePath),
                    isFineWine: IsFineWine);

                ResetForm();

                StatusMessage = "Vinet har sparats!";
            }
        }
        catch (InvalidDataException)
        {
            StatusMessage =
                "Bilden måste vara en giltig JPG- eller PNG-fil och högst 10 MB.";
        }
        catch (ArgumentException)
        {
            StatusMessage =
                "Kontrollera vinets uppgifter och bildformat.";
        }
        catch (IOException)
        {
            StatusMessage =
                "Det gick inte att läsa eller spara bildfilen.";
        }
        catch (Exception)
        {
            StatusMessage = "Det gick inte att spara vinet.";
        }
    }

    private void ResetForm()
    {
        WineName = string.Empty;
        WineDescription = string.Empty;
        WineProducer = string.Empty;
        WineCountry = string.Empty;
        WineRegion = string.Empty;
        SelectedWineType = WineType.Red;
        CustomWineType = string.Empty;
        WineGrape = string.Empty;
        WineYear = string.Empty;
        AlcoholPercentage = string.Empty;
        RecommendedFood = string.Empty;
        IsFineWine = false;

        CurrentImageReference = null;
        SelectedImagePath = null;
        SelectedImageName = "Ingen bild vald";
    }
}
