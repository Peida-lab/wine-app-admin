using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using WineApp.Application;
using WineApp.Infrastructure;


namespace WineApp.Presentation;


public partial class App : Microsoft.UI.Xaml.Application
{
    private readonly IServiceProvider _serviceProvider;
    private Window? _window;


    public App()
    {
        InitializeComponent();

        ServiceCollection service = new();

        service.AddApplication();
        service.AddInfrastructure();

        service.AddSingleton<MainWindow>();
        _serviceProvider = service.BuildServiceProvider();
    }


    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = _serviceProvider.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
