
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Domain.Wines.Enums;
using WineApp.Presentation.Navigation;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class WinesPage : Page
{
    private readonly INavigationService _navigationService;

    public WinesPageViewModel ViewModel { get; }

    public WinesPage(
        WinesPageViewModel viewModel,
        INavigationService navigationService)
    {
        ViewModel = viewModel;
        _navigationService = navigationService;

        InitializeComponent();

        Loaded += WinesPage_Loaded;
    }

    private async void WinesPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await ViewModel.LoadWinesAsync();
    }

    private void EditWine_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is Button button &&
            button.CommandParameter is Guid wineId)
        {
            _navigationService.Navigate(
                AppPage.WineEditor,
                wineId);
        }
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
            WineType.Other =>
                string.IsNullOrWhiteSpace(customWineType)
                    ? "Övrigt"
                    : customWineType,
            _ => "Okänd vintyp"
        };
    }
}
