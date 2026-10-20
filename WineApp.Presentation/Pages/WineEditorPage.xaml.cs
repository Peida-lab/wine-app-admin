
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class WineEditorPage : Page
{
    private readonly MainWindow _mainWindow;

    public WineEditorViewModel ViewModel { get; }

    public Guid? WineId { get; set; }

    public WineEditorPage(
        WineEditorViewModel viewModel,
        MainWindow mainWindow)
    {
        ViewModel = viewModel;
        _mainWindow = mainWindow;

        InitializeComponent();

        Loaded += WineEditorPage_Loaded;
    }

    private async void WineEditorPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        await ViewModel.LoadWineAsync(WineId);
    }

    private async void ChooseImage_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker(_mainWindow.AppWindow.Id)
            {
                FileTypeFilter =
                {
                    ".jpg",
                    ".jpeg",
                    ".png"
                }
            };

            var file = await picker.PickSingleFileAsync();

            if (file is null)
                return;

            // Send the selected image to the ViewModel
            ViewModel.SetSelectedImage(file.Path);
        }
        catch (Exception)
        {
            ViewModel.StatusMessage =
                "Det gick inte att välja bilden.";
        }
    }
}
