using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Presentation.Navigation;

namespace WineApp.Presentation;

public sealed partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;

    public MainWindow(INavigationService navigationService)
    {
        InitializeComponent();

        _navigationService = navigationService;

        _navigationService.Initialize(ContentFrame);

        MainNavigation.SelectedItem = MainNavigation.MenuItems[0];

        MainNavigation.SelectionChanged += MainNavigation_SelectionChanged;

        _navigationService.Navigate(AppPage.Home);
    }

    private void MainNavigation_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem selectedItem)
            return;

        if (selectedItem.Tag is not string tag)
            return;

        if (Enum.TryParse<AppPage>(tag, out var page))
        {
            _navigationService.Navigate(page);
        }
    }
}