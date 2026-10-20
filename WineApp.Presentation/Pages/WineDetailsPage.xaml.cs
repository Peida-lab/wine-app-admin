
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
        await ViewModel.LoadWineAsync(WineId);
    }

    // Return to the wine list
    private void Back_Click(
        object sender,
        RoutedEventArgs e)
    {
        _navigationService.Navigate(AppPage.Wines);
    }

    // Open the existing wine editor
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
