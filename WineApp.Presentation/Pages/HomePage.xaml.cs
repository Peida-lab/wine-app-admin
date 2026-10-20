
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Domain.Wines.Models;
using WineApp.Presentation.Navigation;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class HomePage : Page
{
    private readonly INavigationService _navigationService;

    public HomePageViewModel ViewModel { get; }

    public HomePage(
        HomePageViewModel viewModel,
        INavigationService navigationService)
    {
        ViewModel = viewModel;
        _navigationService = navigationService;

        InitializeComponent();

        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await ViewModel.LoadWinesAsync();
    }

    // Open wine details when a row is clicked
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

    // Open the editor for the selected wine
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
}
