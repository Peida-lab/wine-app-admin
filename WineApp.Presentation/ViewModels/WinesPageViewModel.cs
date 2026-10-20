
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WineApp.Application.Wines;
using WineApp.Domain.Wines.Enums;
using WineApp.Domain.Wines.Models;

namespace WineApp.Presentation.ViewModels;

public partial class WinesPageViewModel(IWineService wineService)
    : ObservableObject
{
    private List<Wine> _allWines = [];

    public ObservableCollection<Wine> Wines { get; } = [];

    public IReadOnlyList<string> WineTypeFilters { get; } =
    [
        "Alla vintyper",
        "Rött vin",
        "Vitt vin",
        "Rosévin",
        "Mousserande",
        "Dessertvin",
        "Övrigt"
    ];

    public IReadOnlyList<string> FineWineFilters { get; } =
    [
        "Alla viner",
        "Fine Wines",
        "Hus viner"
    ];

    private static readonly WineType?[] WineTypeValues =
    [
        null,
        WineType.Red,
        WineType.White,
        WineType.Rose,
        WineType.Sparkling,
        WineType.Dessert,
        WineType.Other
    ];

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int SelectedWineTypeIndex { get; set; }

    [ObservableProperty]
    public partial int SelectedFineWineIndex { get; set; }

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
            _allWines = await wineService.GetAllWinesAsync();
            FilterWines();
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
    public async Task DeleteWineAsync(Guid wineId)
    {
        ErrorMessage = null;

        try
        {
            await wineService.DeleteWineAsync(wineId);

            // Refresh the wine list after deletion
            await LoadWinesAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Det gick inte att ta bort vinet.";
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterWines();
    }

    partial void OnSelectedWineTypeIndexChanged(int value)
    {
        FilterWines();
    }

    partial void OnSelectedFineWineIndexChanged(int value)
    {
        FilterWines();
    }

    private void FilterWines()
    {
        string search = SearchText.Trim();

        WineType? selectedType =
            SelectedWineTypeIndex >= 0 &&
            SelectedWineTypeIndex < WineTypeValues.Length
                ? WineTypeValues[SelectedWineTypeIndex]
                : null;

        var filteredWines = _allWines.Where(wine =>
        {
            bool matchesSearch =
                string.IsNullOrEmpty(search) ||
                wine.WineName.Value.Contains(
                    search, StringComparison.OrdinalIgnoreCase) ||
                wine.WineProducer.Value.Contains(
                    search, StringComparison.OrdinalIgnoreCase) ||
                wine.WineCountry.Value.Contains(
                    search, StringComparison.OrdinalIgnoreCase) ||
                wine.WineGrape.Value.Contains(
                    search, StringComparison.OrdinalIgnoreCase) ||
                (wine.WineRegion?.Contains(
                    search, StringComparison.OrdinalIgnoreCase) ?? false);

            bool matchesWineType =
                selectedType is null ||
                wine.WineType == selectedType.Value;

            bool matchesFineWine = SelectedFineWineIndex switch
            {
                1 => wine.IsFineWine,
                2 => !wine.IsFineWine,
                _ => true
            };

            return matchesSearch &&
                   matchesWineType &&
                   matchesFineWine;
        });

        Wines.Clear();

        foreach (var wine in filteredWines)
        {
            Wines.Add(wine);
        }
    }
}
