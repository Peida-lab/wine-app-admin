
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using WineApp.Application.Wines;
using WineApp.Domain.Wines.Enums;

namespace WineApp.Presentation.ViewModels;

public partial class WineEditorViewModel(IWineService wineService)
    : ObservableObject
{
    private readonly IWineService _wineService = wineService;

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

    public IReadOnlyList<WineType> WineTypes { get; } =
        Enum.GetValues<WineType>();

    [RelayCommand]
    private async Task SaveWineAsync()
    {
        StatusMessage = string.Empty;

        // Validate required fields
        if (string.IsNullOrWhiteSpace(WineName) ||
            string.IsNullOrWhiteSpace(WineDescription) ||
            string.IsNullOrWhiteSpace(WineProducer) ||
            string.IsNullOrWhiteSpace(WineCountry) ||
            string.IsNullOrWhiteSpace(WineGrape))
        {
            StatusMessage = "Fyll i alla obligatoriska fält.";
            return;
        }

        // Validate year
        if (!int.TryParse(WineYear, out int year))
        {
            StatusMessage = "Ange en giltig årgång.";
            return;
        }

        // Validate alcohol percentage
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
                isFineWine: IsFineWine);

            ResetForm();

            StatusMessage = "Vinet har sparats!";
        }
        catch (ArgumentException)
        {
            StatusMessage = "Kontrollera vinets uppgifter och försök igen.";
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
    }
}
