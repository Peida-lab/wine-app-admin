
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

    private static readonly StringComparer SwedishComparer =
        StringComparer.Create(
            CultureInfo.GetCultureInfo("sv-SE"),
            ignoreCase: true);

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

    // Available sorting options
    public IReadOnlyList<string> SortOptions { get; } =
    [
        "Namn A–Ö",
        "Namn Ö–A",
        "Nyaste årgång först",
        "Äldsta årgång först"
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
    public partial int SelectedSortIndex { get; set; }

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

            // Refresh the list after deletion
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

    // Update the list whenever sorting changes
    partial void OnSelectedSortIndexChanged(int value)
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

        // First apply search and filters
        IEnumerable<Wine> filteredWines = _allWines.Where(wine =>
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

        // Then sort the filtered results
        IEnumerable<Wine> sortedWines = SelectedSortIndex switch
        {
            1 => filteredWines
                .OrderByDescending(
                    wine => wine.WineName.Value,
                    SwedishComparer),

            2 => filteredWines
                .OrderByDescending(
                    wine => wine.WineYear.Value)
                .ThenBy(
                    wine => wine.WineName.Value,
                    SwedishComparer),

            3 => filteredWines
                .OrderBy(
                    wine => wine.WineYear.Value)
                .ThenBy(
                    wine => wine.WineName.Value,
                    SwedishComparer),

            _ => filteredWines
                .OrderBy(
                    wine => wine.WineName.Value,
                    SwedishComparer)
        };

        // Refresh the visible collection
        Wines.Clear();

        foreach (Wine wine in sortedWines)
        {
            Wines.Add(wine);
        }
    }
}
