
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using WineApp.Application;
using WineApp.Infrastructure;
using WineApp.Presentation.Navigation;
using WineApp.Presentation.Pages;
using WineApp.Presentation.ViewModels;

namespace WineApp.Presentation;

public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IServiceProvider _serviceProvider;
    private Window? _window;

    public App()
    {
        InitializeComponent();

        ServiceCollection services = new();

        services.AddApplication();
        services.AddInfrastructure();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddTransient<HomePageViewModel>();
        services.AddTransient<WineEditorViewModel>();

        // Pages
        services.AddTransient<HomePage>();
        services.AddTransient<WinesPage>();
        services.AddTransient<WineEditorPage>();

        // MainWindow
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = _serviceProvider.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
