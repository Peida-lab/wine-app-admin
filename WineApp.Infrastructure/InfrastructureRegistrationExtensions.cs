
using Microsoft.Extensions.DependencyInjection;
using WineApp.Application.Images;
using WineApp.Application.Wines.Interfaces;
using WineApp.Infrastructure.Images;
using WineApp.Infrastructure.Wines;

namespace WineApp.Infrastructure;

public static class InfrastructureRegistrationExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        string folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WineApp");

        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, "wines.json");
        string imagesFolder = Path.Combine(folderPath, "images");

        services.AddSingleton<IWineRepository>(
            _ => new JsonWineRepository(filePath));

        services.AddSingleton<IImageStorageService>(
            _ => new LocalImageStorageService(imagesFolder));

        return services;
    }
}
