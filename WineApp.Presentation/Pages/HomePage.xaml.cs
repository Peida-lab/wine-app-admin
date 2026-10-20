
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
    private readonly WinesPageViewModel _winesPageViewModel;

    public HomePageViewModel ViewModel { get; }

    public HomePage(
        HomePageViewModel viewModel,
        WinesPageViewModel winesPageViewModel,
        INavigationService navigationService)
    {
        ViewModel = viewModel;
        _winesPageViewModel = winesPageViewModel;
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

    // Open all wines
    private void AllWines_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenWineList(
            wineTypeIndex: 0,
            fineWineIndex: 0);
    }

    // Open red wines
    private void RedWines_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenWineList(
            wineTypeIndex: 1,
            fineWineIndex: 0);
    }

    // Open white wines
    private void WhiteWines_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenWineList(
            wineTypeIndex: 2,
            fineWineIndex: 0);
    }

    // Open Fine Wines
    private void FineWines_Click(
        object sender,
        RoutedEventArgs e)
    {
        OpenWineList(
            wineTypeIndex: 0,
            fineWineIndex: 1);
    }

    // Apply the appropriate filters before navigation
    private void OpenWineList(
        int wineTypeIndex,
        int fineWineIndex)
    {
        _winesPageViewModel.ApplyDashboardFilter(
            wineTypeIndex,
            fineWineIndex);

        _navigationService.Navigate(AppPage.Wines);
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
