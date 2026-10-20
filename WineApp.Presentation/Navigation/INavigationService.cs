using Microsoft.UI.Xaml.Controls;

namespace WineApp.Presentation.Navigation;

public interface INavigationService
{
    void Initialize(Frame frame);
    void Navigate(AppPage page);
}
