
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class WineEditorPage : Page
{
    public WineEditorViewModel ViewModel { get; }

    public Guid? WineId { get; set; }

    public WineEditorPage(WineEditorViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();

        Loaded += WineEditorPage_Loaded;
    }

    private async void WineEditorPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await ViewModel.LoadWineAsync(WineId);
    }
}
