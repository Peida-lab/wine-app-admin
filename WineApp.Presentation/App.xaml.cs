using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;


namespace WineApp.Presentation;


public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private Window? _window;


    public App()
    {
        InitializeComponent();

        ServiceCollection service = new();

        service.AddSingleton<MainWindow>();
        _serviceProvider = service.BuildServiceProvider();
    }


    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = _serviceProvider.GetRequiredService<MainWindow>();
        _window.Activate();
    }
}
