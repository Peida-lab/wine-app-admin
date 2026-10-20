
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

    // Open the read-only details page
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

    // Navigate to the wine editor
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

    // Confirm before deleting a wine
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

    // Convert wine types to Swedish display names
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
