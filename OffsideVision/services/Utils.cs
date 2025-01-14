using System.Drawing;
using System.IO;
using System.Windows.Documents;
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

}