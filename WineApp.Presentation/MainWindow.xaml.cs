
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

        MainNavigation.ItemInvoked += MainNavigation_ItemInvoked;

        _navigationService.Navigate(AppPage.Home);
    }

    private void MainNavigation_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is not NavigationViewItem selectedItem)
            return;

        if (selectedItem.Tag is not string tag)
            return;

        if (Enum.TryParse<AppPage>(tag, out var page))
        {
            _navigationService.Navigate(page);
        }
    }
}
