using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OffsideVision;

public partial class ResultWindow : Window
{
    public ResultWindow(BitmapImage imageSource)
    {
        InitializeComponent();
        ResultImage.Source = imageSource;
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