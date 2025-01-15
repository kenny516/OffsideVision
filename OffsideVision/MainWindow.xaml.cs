using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using OffsideVision.model;
using OffsideVision.services;

namespace OffsideVision;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // setup for the design window
    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    //
    public void UploadButton_Click(Object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            FilePathTextBox.Text = openFileDialog.FileName;
        }
    }

    public void ProcessButton_Click(Object sender, RoutedEventArgs e)
    {
        string filePath = FilePathTextBox.Text;
        if (filePath.Equals("Aucun fichier sélectionné..."))
        {
            MessageBox.Show("Please select an image file to process.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        BitmapImage bitmap = new BitmapImage(new Uri(filePath));
        Bitmap image = Utils.ConvertBitmapImageToBitmap(bitmap);
        List<Circle> circles = HandlerOffside.CircleDetect(image, 5, 50);

        Console.WriteLine("Nombre de cercle :" + circles.Count);

        Circle carrier = CircleAnalyzer.GetCarrier(circles);
        Circle ball = CircleAnalyzer.Getball(circles);
        
        //Circle carrierDiff = CircleAnalyzer.GEtClosestCircleDiff(circles,ball, carrier);
        //List<Circle> AttackerOffside = CircleAnalyzer.GetOffsideTeam(circles,carrierDiff);
        
        List<Circle> AttackerOffside = CircleAnalyzer.GetOffsideCircles(circles);
        
        image = HandlerOffside.AnnotateImage(image, circles, AttackerOffside);
        HandlerOffside.DrawLine(image, 50);
        ResultWindow resultWindow = new ResultWindow(bitmap,image);
        resultWindow.Show();
        MessageBox.Show("Processing image...", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}