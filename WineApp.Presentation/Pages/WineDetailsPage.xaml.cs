
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Presentation.Navigation;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class WineDetailsPage : Page
{
    private readonly INavigationService _navigationService;

    public WineDetailsViewModel ViewModel { get; }

    public Guid? WineId { get; set; }

    // The page to return to
    public AppPage ReturnPage { get; set; } = AppPage.Wines;

    public WineDetailsPage(
        WineDetailsViewModel viewModel,
        INavigationService navigationService)
    {
        ViewModel = viewModel;
        _navigationService = navigationService;

        InitializeComponent();

        Loaded += WineDetailsPage_Loaded;
    }

    private async void WineDetailsPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        // Update back button text based on previous page
        BackButton.Content = ReturnPage == AppPage.Home
            ? "Tillbaka till översikt"
            : "Tillbaka till alla viner";

        await ViewModel.LoadWineAsync(WineId);
    }

    // Return to the page the user came from
    private void Back_Click(
        object sender,
        RoutedEventArgs e)
    {
        _navigationService.Navigate(ReturnPage);
    }

    // Open existing wine editor
    private void EditWine_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!ViewModel.IsWineLoaded || !WineId.HasValue)
            return;

        _navigationService.Navigate(
            AppPage.WineEditor,
            WineId.Value);
    }
}
