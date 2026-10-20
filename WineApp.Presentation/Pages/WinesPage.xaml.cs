
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WineApp.Domain.Wines.Enums;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class WinesPage : Page
{
    public WinesPageViewModel ViewModel { get; }

    public WinesPage(WinesPageViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();

        Loaded += WinesPage_Loaded;
    }

    private async void WinesPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await ViewModel.LoadWinesAsync();
    }

    public static string GetWineTypeDisplay(
        WineType wineType,
        string? customWineType)
    {
        return wineType switch
        {
            WineType.Red => "Rött vin",
            WineType.White => "Vitt vin",
            WineType.Rose => "Rosévin",
            WineType.Sparkling => "Mousserande",
            WineType.Dessert => "Dessertvin",
            WineType.Other => string.IsNullOrWhiteSpace(customWineType)
                ? "Övrigt"
                : customWineType,
            _ => "Okänd vintyp"
        };
    }
}
