
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Presentation.Pages;

namespace WineApp.Presentation.Navigation;

public class NavigationService(IServiceProvider serviceProvider)
    : INavigationService
{
    private Frame? _frame;

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate(AppPage page, Guid? wineId = null)
    {
        if (_frame is null)
        {
            throw new InvalidOperationException(
                "Navigeringen har inte initierats.");
        }

        Page targetPage = page switch
        {
            AppPage.Home =>
                serviceProvider.GetRequiredService<HomePage>(),

            AppPage.Wines =>
                serviceProvider.GetRequiredService<WinesPage>(),

            AppPage.WineEditor =>
                serviceProvider.GetRequiredService<WineEditorPage>(),

            _ => throw new ArgumentOutOfRangeException(nameof(page))
        };

        // Pass the wine ID to the editor page
        if (targetPage is WineEditorPage editorPage)
        {
            editorPage.WineId = wineId;
        }

        _frame.Content = targetPage;
    }
}
