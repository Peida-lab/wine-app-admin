
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        "Husviner"
    ];

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

    [ObservableProperty]
    public partial string ResultSummary { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string EmptyStateMessage { get; set; } =
        string.Empty;

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

            _allWines = [];
            Wines.Clear();
            ResultSummary = string.Empty;
            EmptyStateMessage = string.Empty;
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

            await LoadWinesAsync();
        }
        catch (Exception)
        {
            ErrorMessage = "Det gick inte att ta bort vinet.";
        }
    }

    // Apply filters when a dashboard card is clicked
    public void ApplyDashboardFilter(
        int wineTypeIndex,
        int fineWineIndex)
    {
        // Clear an old search so the selected category
        // always displays all matching wines.
        SearchText = string.Empty;

        SelectedWineTypeIndex = wineTypeIndex;
        SelectedFineWineIndex = fineWineIndex;

        // Start with alphabetical sorting
        SelectedSortIndex = 0;

        FilterWines();
    }

    // Reset search, filters and sorting
    [RelayCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedWineTypeIndex = 0;
        SelectedFineWineIndex = 0;
        SelectedSortIndex = 0;

        FilterWines();
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

    partial void OnSelectedSortIndexChanged(int value)
    {
        FilterWines();
    }

    private void FilterWines()
    {
        string search = (SearchText ?? string.Empty).Trim();

        WineType? selectedType =
            SelectedWineTypeIndex >= 0 &&
            SelectedWineTypeIndex < WineTypeValues.Length
                ? WineTypeValues[SelectedWineTypeIndex]
                : null;

        // Search and filter
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

        // Sort the filtered wines
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

        Wines.Clear();

        foreach (Wine wine in sortedWines)
        {
            Wines.Add(wine);
        }

        // Result counter
        ResultSummary =
            $"Visar {Wines.Count} av {_allWines.Count} viner";

        // Empty state
        if (Wines.Count > 0)
        {
            EmptyStateMessage = string.Empty;
        }
        else if (_allWines.Count == 0)
        {
            EmptyStateMessage =
                "Inga viner har registrerats ännu.";
        }
        else
        {
            EmptyStateMessage =
                "Inga viner matchar din sökning eller dina filter.";
        }
    }
}
