
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using WineApp.Presentation.Pages;

namespace WineApp.Presentation.Navigation;

public class NavigationService(IServiceProvider serviceProvider)
    : INavigationService
{
    private Frame? _frame;

    // Remember the currently displayed page
    private AppPage? _currentPage;

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

            AppPage.WineDetails =>
                serviceProvider.GetRequiredService<WineDetailsPage>(),

            _ => throw new ArgumentOutOfRangeException(nameof(page))
        };

        // Pass wine ID to the editor
        if (targetPage is WineEditorPage editorPage)
        {
            editorPage.WineId = wineId;
        }

        // Pass wine ID and remember where the user came from
        if (targetPage is WineDetailsPage detailsPage)
        {
            detailsPage.WineId = wineId;

            detailsPage.ReturnPage =
                _currentPage == AppPage.Home
                    ? AppPage.Home
                    : AppPage.Wines;
        }

        _frame.Content = targetPage;

        // Remember the current page for the next navigation
        _currentPage = page;
    }
}
