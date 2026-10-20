
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

    // Select a new image
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

            ViewModel.SetSelectedImage(file.Path);
        }
        catch (Exception)
        {
            ViewModel.StatusMessage =
                "Det gick inte att välja bilden.";
        }
    }

    // Confirm before removing a saved image
    private async void RemoveImage_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!ViewModel.CanRemoveSavedImage)
            return;

        ContentDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            Title = "Ta bort bild?",
            Content =
                "Är du säker på att du vill ta bort bilden från vinet? " +
                "Bilden tas bort direkt och åtgärden kan inte ångras. " +
                "En eventuell nyvald bild rensas också.",
            PrimaryButtonText = "Ta bort bild",
            CloseButtonText = "Avbryt",
            DefaultButton = ContentDialogButton.Close
        };

        ContentDialogResult result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.RemoveWineImageAsync();
        }
    }
}
