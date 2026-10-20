
using Microsoft.UI.Xaml.Controls;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation.Pages;

public sealed partial class WineEditorPage : Page
{
    public WineEditorViewModel ViewModel { get; }

    public WineEditorPage(WineEditorViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();
    }
}
