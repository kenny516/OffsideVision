using System.Drawing;
using System.Windows.Documents;
using OffsideVision.model;

namespace OffsideVision.services;

public class HandlePixel
{
    // static Color Team1Color = Color.FromArgb(250, 105, 105);
    // static Color Team2Color = Color.FromArgb(86, 162, 232);
    // static Color ballColor = Color.FromArgb(18, 18, 18);
    static Color Team1Color = Color.Red;
    static Color Team2Color = Color.FromArgb(25, 118, 210);
    static Color ballColor = Color.Black;

    
    public static List<Circle> ColorDetect(Bitmap image)
    {
        List<Circle> circles = new List<Circle>();
        bool[,] visited = new bool[image.Width, image.Height]; // Marquer les pixels déjà traités

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                if (visited[x, y]) continue; // Passer si déjà traité

                Color pixelColor = image.GetPixel(x, y);

                // Vérification pour rouge, bleu ou noir
                if (Utils.IsCloseColor(pixelColor, Team1Color) ||
                    Utils.IsCloseColor(pixelColor, Team2Color) ||
                    Utils.IsCloseColor(pixelColor, ballColor))
                {
                    Circle currentPixel = new Circle();
                    currentPixel.X = x;
                    currentPixel.Y = y;
                    currentPixel.Radius = 1;
                    currentPixel.Color = Utils.IsCloseColor(pixelColor, Team1Color) ? "Red" :
                        Utils.IsCloseColor(pixelColor, Team2Color) ? "Blue" : "Black";

                    if (currentPixel != null)
                    {
                        circles.Add(currentPixel);
                    }
                }
            }
        }
        return circles;
    }
    
}