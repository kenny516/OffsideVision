using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using OffsideVision.services;

namespace OffsideVision;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // setup for the design window
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed) return;
        DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
            FilePathTextBox.Text = openFileDialog.FileName;
    }

    private void ProcessButton_Click(object sender, RoutedEventArgs e)
    {
        var filePath = FilePathTextBox.Text;
        if (filePath.Equals("Aucun fichier sélectionné..."))
        {
            MessageBox.Show("Please select an image file to process.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        var bitmapImage = new BitmapImage(new Uri(filePath));
        var bitmap = Utils.ConvertBitmapImageToBitmap(bitmapImage);

        var centreY = (int)bitmapImage.Height / 2;
        Console.WriteLine("CentreY :" + centreY);

        var lastDefenseurY = 0;

        var circles = HandlerOffside.CircleDetect(bitmap, 5, 50);

        Console.WriteLine("Nombre de cercle :" + circles.Count);


        // Circle carrier = CircleAnalyzer.GetCarrier(circles);
        // Circle ball = CircleAnalyzer.Getball(circles);
        //Circle carrierDiff = CircleAnalyzer.GEtClosestCircleDiff(circles,ball, carrier);
        //List<Circle> AttackerOffside = CircleAnalyzer.GetOffsideTeam(circles,carrierDiff,centreY);

        var attackerOffside = CircleAnalyzer.GetOffsideCircles(ref lastDefenseurY, circles, centreY);
        
        HandlerOffside.DrawLine(bitmap, lastDefenseurY);
        bitmap = HandlerOffside.AnnotateImage(bitmap, circles, attackerOffside);
        var resultWindow = new ResultWindow(bitmapImage, bitmap);
        resultWindow.Show();
    }
}