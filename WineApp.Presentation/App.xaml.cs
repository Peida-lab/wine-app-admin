using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using WineApp.Application;
using WineApp.Infrastructure;
using WineApp.Presentation.Navigation;
using WineApp.Presentation.Pages;


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
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddTransient<HomePage>();
        services.AddTransient<WinesPage>();
        services.AddTransient<WineEditorPage>();
        

        services.AddSingleton<MainWindow>();
        _serviceProvider = services.BuildServiceProvider();
    }


    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = _serviceProvider.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
