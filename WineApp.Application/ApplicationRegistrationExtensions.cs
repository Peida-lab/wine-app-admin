using Microsoft.Extensions.DependencyInjection;
using WineApp.Application.Wines;

namespace WineApp.Application;

public static class ApplicationRegistrationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IWineService, WineService>();
        return services;
    }
}
