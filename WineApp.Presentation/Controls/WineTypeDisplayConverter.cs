
using Microsoft.UI.Xaml.Data;
using System;
using WineApp.Domain.Wines.Enums;

namespace WineApp.Presentation.Controls;

public class WineTypeDisplayConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        if (value is not WineType wineType)
            return string.Empty;

        return wineType switch
        {
            WineType.Red => "Rött vin",
            WineType.White => "Vitt vin",
            WineType.Rose => "Rosévin",
            WineType.Sparkling => "Mousserande",
            WineType.Dessert => "Dessertvin",
            WineType.Other => "Övrigt",
            _ => "Okänd vintyp"
        };
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        string language)
    {
        throw new NotSupportedException();
    }
}
