
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WineApp.Presentation.Controls;

public class WineImage : Grid
{
    private readonly Image _image = new()
    {
        Stretch = Stretch.Uniform,
        Visibility = Visibility.Collapsed
    };

    private readonly SymbolIcon _placeholder = new(Symbol.Pictures)
    {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };

    private int _loadVersion;

    public WineImage()
    {
        Children.Add(_placeholder);
        Children.Add(_image);

        Loaded += WineImage_Loaded;
        Unloaded += WineImage_Unloaded;

        _image.ImageFailed += (_, _) =>
        {
            _image.Visibility = Visibility.Collapsed;
            _placeholder.Visibility = Visibility.Visible;
        };
    }

    // Filename saved in wines.json
    public string? ImageReference
    {
        get => (string?)GetValue(ImageReferenceProperty);
        set => SetValue(ImageReferenceProperty, value);
    }

    public static readonly DependencyProperty ImageReferenceProperty =
        DependencyProperty.Register(
            nameof(ImageReference),
            typeof(string),
            typeof(WineImage),
            new PropertyMetadata(null, OnImageChanged));

    // Absolute path of a newly selected image
    public string? SelectedImagePath
    {
        get => (string?)GetValue(SelectedImagePathProperty);
        set => SetValue(SelectedImagePathProperty, value);
    }

    public static readonly DependencyProperty SelectedImagePathProperty =
        DependencyProperty.Register(
            nameof(SelectedImagePath),
            typeof(string),
            typeof(WineImage),
            new PropertyMetadata(null, OnImageChanged));

    private static void OnImageChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs args)
    {
        if (sender is WineImage control && control.IsLoaded)
        {
            _ = control.LoadImageAsync();
        }
    }

    private void WineImage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        _ = LoadImageAsync();
    }

    private void WineImage_Unloaded(
        object sender,
        RoutedEventArgs e)
    {
        _loadVersion++;
        _image.Source = null;
    }

    private async Task LoadImageAsync()
    {
        int currentVersion = ++_loadVersion;

        // Display placeholder until a valid image loads
        _image.Source = null;
        _image.Visibility = Visibility.Collapsed;
        _placeholder.Visibility = Visibility.Visible;

        string? imagePath = GetImagePath();

        if (string.IsNullOrWhiteSpace(imagePath) ||
            !File.Exists(imagePath))
        {
            return;
        }

        try
        {
            StorageFile file =
                await StorageFile.GetFileFromPathAsync(imagePath);

            using IRandomAccessStream stream =
                await file.OpenAsync(FileAccessMode.Read);

            BitmapImage bitmap = new()
            {
                DecodePixelWidth = 500
            };

            await bitmap.SetSourceAsync(stream);

            // Prevent an old image from replacing a newer selection
            if (!IsLoaded || currentVersion != _loadVersion)
                return;

            _image.Source = bitmap;
            _image.Visibility = Visibility.Visible;
            _placeholder.Visibility = Visibility.Collapsed;
        }
        catch (Exception)
        {
            // Keep placeholder visible when image loading fails
            if (currentVersion == _loadVersion)
            {
                _image.Source = null;
                _image.Visibility = Visibility.Collapsed;
                _placeholder.Visibility = Visibility.Visible;
            }
        }
    }

    private string? GetImagePath()
    {
        // Preview a newly selected image first
        if (!string.IsNullOrWhiteSpace(SelectedImagePath))
            return SelectedImagePath;

        if (string.IsNullOrWhiteSpace(ImageReference))
            return null;

        // Image references must be filenames, not paths
        string fileName = Path.GetFileName(ImageReference);

        if (fileName != ImageReference)
            return null;

        return Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "WineApp",
            "images",
            fileName);
    }
}
