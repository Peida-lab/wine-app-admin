
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WineApp.Application.Wines;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;

namespace WineApp.Presentation.ViewModels;

public partial class HomePageViewModel(IWineService wineService)
    : ObservableObject
{
    [ObservableProperty]
    public partial int TotalWines { get; set; }

    [ObservableProperty]
    public partial int RedWines { get; set; }

    [ObservableProperty]
    public partial int WhiteWines { get; set; }

    [ObservableProperty]
    public partial int FineWines { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    public ObservableCollection<Wine> RecentWines { get; } = [];

    public async Task LoadWinesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var wines = await wineService.GetAllWinesAsync();

            // Update dashboard statistics
            TotalWines = wines.Count;

            RedWines = wines.Count(wine =>
                wine.WineType == WineType.Red);

            WhiteWines = wines.Count(wine =>
                wine.WineType == WineType.White);

            FineWines = wines.Count(wine =>
                wine.IsFineWine);

            // Refresh the five most recently registered wines
            RecentWines.Clear();

            foreach (var wine in wines.TakeLast(5).Reverse())
            {
                RecentWines.Add(wine);
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Det gick inte att hämta vinerna.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Delete a wine and refresh the dashboard
    public async Task DeleteWineAsync(Guid wineId)
    {
        ErrorMessage = null;

        try
        {
            await wineService.DeleteWineAsync(wineId);

            // Reload both statistics and recent wines
            await LoadWinesAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Det gick inte att ta bort vinet.";
        }
    }
}
