using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

namespace WineApp.Presentation.Navigation;

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private Frame? _frame;

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate(AppPage page)
    {
        if (_frame is null)
        {
            throw new InvalidOperationException(
                "Navigeringen har inte initierats.");
        }

        var targetPage = page switch
        {
            AppPage.Home => serviceProvider.GetRequiredService<HomePage>(),
            AppPage.Wines => serviceProvider.GetRequiredService<WinesPage>(),
            AppPage.WineEditor => serviceProvider.GetRequiredService<WineEditorPage>(),
            _ => throw new ArgumentOutOfRangeException(nameof(page))
        };

        _frame.Content = targetPage;
    }
}