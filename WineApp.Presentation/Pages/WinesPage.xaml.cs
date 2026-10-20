
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;
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

    // Open wine details
    private void Wine_ItemClick(
        object sender,
        ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Wine wine)
            return;

        _navigationService.Navigate(
            AppPage.WineDetails,
            wine.WineId);
    }

    // Open wine editor
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

    // Confirm deletion
    private async void DeleteWine_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not Guid wineId)
        {
            return;
        }

        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Title = "Ta bort vin?",
            Content =
                "Är du säker på att du vill ta bort det här vinet? " +
                "Åtgärden kan inte ångras.",
            PrimaryButtonText = "Ta bort",
            CloseButtonText = "Avbryt",
            DefaultButton = ContentDialogButton.Close
        };

        ContentDialogResult result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.DeleteWineAsync(wineId);
        }
    }

    // Swedish display names
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

    // Show the blue label only for Fine Wines
    public static Visibility GetFineWineVisibility(
        bool isFineWine)
    {
        return isFineWine
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
