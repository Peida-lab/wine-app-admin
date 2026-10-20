using Microsoft.UI.Xaml.Controls;
using System;

namespace WineApp.Presentation.Navigation;

public interface INavigationService
{
    void Initialize(Frame frame);

    void Navigate(AppPage page, Guid? wineId = null);
}