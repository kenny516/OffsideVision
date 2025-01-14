using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OffsideVision;

public partial class ResultWindow : Window
{
    public ResultWindow(Bitmap imageSource)
    {
        InitializeComponent();
        var bitmapImage = new BitmapImage();
        using (var stream = new MemoryStream())
        {
            // Sauvegarder le Bitmap dans un flux mémoire
            imageSource.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);

            // Charger le flux dans le BitmapImage
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
        }
        ResultImage.Source = bitmapImage;
    }
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // Méthode pour afficher/masquer l'indicateur de chargement
    public void SetLoadingState(bool isLoading)
    {
        LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
    }
}