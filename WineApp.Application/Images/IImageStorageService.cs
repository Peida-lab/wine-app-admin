
namespace WineApp.Application.Images;

public interface IImageStorageService
{
    Task<string> SaveImageAsync(
        Stream imageStream,
        string fileName);

    Task DeleteImageAsync(string imageReference);
}
