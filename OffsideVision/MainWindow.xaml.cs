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
        List<Circle> circles = HandlerOffside.CircleDetect(image, 5, 30);

        Console.WriteLine("Nombre de cercle " + circles.Count);

        for (int i = 0; i < circles.Count; i++)
        {
            Console.WriteLine("X" + circles[i].X);
            Console.WriteLine("Y" + circles[i].Y);
            Console.WriteLine("Color" + circles[i].Color);
            Console.WriteLine("radius" + circles[i].Radius);
        }
        HandlerOffside.CheckOffside(circles);
        Circle ball = circles.FirstOrDefault(c=>c.Color == "black");
        Circle circle = CircleAnalyzer.GetectCircleClosest(circles,ball);
        Console.WriteLine("Closest team couleur" + circle.Color);
        
        Circle goalKeeper = CircleAnalyzer.GetectGoalKeeper(circles, "Blue");
        List<Circle> TeamBleu = CircleAnalyzer.GetSameTeam(circles, goalKeeper);
        Circle lastDefense = CircleAnalyzer.GetLastDefenseur(TeamBleu, goalKeeper);
        List<Circle> circleDetected = new List<Circle>();
        Console.WriteLine("last x"+ lastDefense.X);
        Console.WriteLine("last Y"+ lastDefense.Y);
        circleDetected.Add(lastDefense);
        image = HandlerOffside.AnnotateImage(image, circles, circleDetected);
            
        ResultWindow resultWindow = new ResultWindow(image);
        resultWindow.Show();
        MessageBox.Show("Processing image...", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}