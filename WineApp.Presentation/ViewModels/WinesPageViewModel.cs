
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WineApp.Application.Wines;
using WineApp.Domain.Wines.Models;

namespace WineApp.Presentation.ViewModels;

public partial class WinesPageViewModel(IWineService wineService)
    : ObservableObject
{
    public ObservableCollection<Wine> Wines { get; } = [];

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    public async Task LoadWinesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var wines = await wineService.GetAllWinesAsync();

            Wines.Clear();

            foreach (var wine in wines)
            {
                Wines.Add(wine);
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
}
