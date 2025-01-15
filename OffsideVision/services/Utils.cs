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

    public static (double hue, double saturation, double brightness) ToHsv(Color color)
    {
        double hue = color.GetHue();
        double saturation = color.GetSaturation();
        double brightness = color.GetBrightness();
        return (hue, saturation, brightness);
    }

    public static bool IsColorInRange(double hue, double saturation, double brightness, string colorName)
    {
        return colorName switch
        {
            // Rouge : marge élargie pour les variations de teinte, saturation et luminosité
            "Red" => (saturation > 0.5 && brightness > 0.2 && 
                      ((hue >= 0 && hue <= 15) || (hue >= 345 && hue <= 360))),

            // Bleu : ajustement de la plage pour inclure des nuances cyan/bleu clair
            "Blue" => (saturation > 0.4 && brightness > 0.2 &&
                       hue >= 190 && hue <= 260),

            // Noir : luminosité très faible et saturation minimale
            "Black" => brightness < 0.3 && saturation < 0.3,

            // Couleur inconnue ou non gérée
            _ => false
        };
    }


    public static string GetColorNameFromHsv(double hue, double saturation, double brightness)
    {
        // Vérification pour la couleur noire (faible luminosité et faible saturation)
        if (brightness < 0.3 && saturation < 0.3)
            return "Black";

        // Détection du rouge (prendre en compte des variations de saturation et de luminosité)
        if (saturation > 0.5 && brightness > 0.2 && 
            ((hue >= 0 && hue <= 15) || (hue >= 345 && hue <= 360)))
            return "Red";

        // Détection du bleu (prendre en compte des variations de saturation et de luminosité)
        if (saturation > 0.4 && brightness > 0.2 &&
            hue >= 190 && hue <= 260)
            return "Blue";

        // Couleur inconnue
        return "Unknown";
    }

}