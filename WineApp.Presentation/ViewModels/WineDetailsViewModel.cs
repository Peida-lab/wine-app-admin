
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using WineApp.Application.Wines;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;

namespace WineApp.Presentation.ViewModels;

public partial class WineDetailsViewModel(
    IWineService wineService)
    : ObservableObject
{
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
    public partial string WineTypeText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineGrape { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineYear { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string AlcoholPercentage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string RecommendedFood { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string FineWineText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string WineIdText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? ImageReference { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsWineLoaded { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public async Task LoadWineAsync(Guid? wineId)
    {
        IsWineLoaded = false;
        ErrorMessage = string.Empty;
        ImageReference = null;

        if (!wineId.HasValue || wineId.Value == Guid.Empty)
        {
            ErrorMessage = "Inget vin har valts.";
            return;
        }

        IsLoading = true;

        try
        {
            Wine? wine =
                await wineService.GetWineByIdAsync(wineId.Value);

            if (wine is null)
            {
                ErrorMessage = "Vinet kunde inte hittas.";
                return;
            }

            WineName = wine.WineName.Value;
            WineDescription = wine.WineDescription.Value;
            WineProducer = wine.WineProducer.Value;
            WineCountry = wine.WineCountry.Value;

            WineRegion = string.IsNullOrWhiteSpace(wine.WineRegion)
                ? "Ej angiven"
                : wine.WineRegion;

            WineTypeText = wine.WineType switch
            {
                WineType.Red => "Rött vin",
                WineType.White => "Vitt vin",
                WineType.Rose => "Rosévin",
                WineType.Sparkling => "Mousserande",
                WineType.Dessert => "Dessertvin",

                WineType.Other =>
                    string.IsNullOrWhiteSpace(wine.CustomWineType)
                        ? "Övrigt"
                        : wine.CustomWineType,

                _ => "Okänd vintyp"
            };

            WineGrape = wine.WineGrape.Value;

            WineYear = wine.WineYear.Value.ToString();

            AlcoholPercentage =
                wine.AlcoholPercentage.Value.ToString(
                    "0.##",
                    CultureInfo.GetCultureInfo("sv-SE")) + " %";

            RecommendedFood =
                string.IsNullOrWhiteSpace(wine.RecommendedFood)
                    ? "Ej angiven"
                    : wine.RecommendedFood;

            FineWineText = wine.IsFineWine
                ? "Fine Wine – säljs endast på flaska"
                : "Husvin";

            WineIdText = wine.WineId.ToString();

            ImageReference = wine.ImageReference;

            IsWineLoaded = true;
        }
        catch (Exception)
        {
            ErrorMessage = "Det gick inte att hämta vinets information.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
