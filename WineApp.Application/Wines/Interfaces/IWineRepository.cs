using WineApp.Domain.Wines.Models;

namespace WineApp.Application.Wines.Interfaces;

public interface IWineRepository
{
    Task AddWineAsync(Wine wine);
    Task<List<Wine>> GetAllWinesAsync();
    Task<Wine?> GetWineByIdAsync(Guid wineId);
    Task UpdateWineAsync(Wine wine);
    Task DeleteWineAsync(Guid wineId);

}
