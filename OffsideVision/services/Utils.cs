using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using OffsideVision.model;

namespace OffsideVision.services;

public class Utils
{
    public static Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
    {
        // Créer un StreamMemory pour stocker les données de l'image
        using (MemoryStream memoryStream = new MemoryStream())
        {
            // Enregistrez l'image dans le StreamMemory au format PNG
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memoryStream);

            // Retourner le Bitmap à partir du StreamMemory
            return new Bitmap(memoryStream);
        }
    }

    public static bool IsCloseColor(Color color1, Color color2, int tolerance = 70)
    {
        return Math.Abs(color1.R - color2.R) < tolerance &&
               Math.Abs(color1.G - color2.G) < tolerance &&
               Math.Abs(color1.B - color2.B) < tolerance;
    }

    public static Circle DetectCircleClosestBall(List<Circle> circles)
    {
        Circle circleClosestBall = new Circle();
        Circle ball = circles.FirstOrDefault((c => c.Color == "black"));
        if (ball == null)
        {
            throw new Exception("Aucun ballon trouver");
        }

        double distanceX = 0;
        double distanceY = 0;
        for (int i = 0; i < circles.Count; i++)
        {
            if (circles[i].Color == "black")
            {
                continue;
            }

            if (distanceX == 0 && distanceY == 0)
            {
                distanceX = Math.Abs(ball.X - circles[i].X);
                distanceY = Math.Abs(ball.Y - circles[i].Y);
                circleClosestBall = circles[i];
            }
            else
            {
                double newDistanceX = Math.Abs(ball.X - circles[i].X);
                double newDistanceY = Math.Abs(ball.Y - circles[i].Y);
                if (newDistanceX < distanceX && newDistanceY < distanceY)
                {
                    distanceX = newDistanceX;
                    distanceY = newDistanceY;
                    circleClosestBall = circles[i];
                }
            }
        }
        return circleClosestBall;
    }
}