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
            // Rouge : De rouge clair à foncé avec différentes saturations et luminosité
            "Red" => (saturation > 0.3 && brightness > 0.1 && 
                      ((hue >= 0 && hue <= 15) || (hue >= 15 && hue <= 30) || (hue >= 330 && hue <= 360))), // Couvrir du rouge clair au rouge foncé

            // Bleu : De bleu clair à foncé
            "Blue" => (saturation > 0.3 && brightness > 0.2 && 
                       ((hue >= 190 && hue <= 220) || (hue >= 220 && hue <= 260))), // Couvrir du bleu clair au bleu foncé

            // Noir : Large plage de gris foncés et noir
            "Black" => (brightness < 0.4 && saturation < 0.3), // Inclure les gris foncés jusqu'au noir pur

            // Couleur inconnue ou non gérée
            _ => false
        };
    }

    public static string GetColorNameFromHsv(double hue, double saturation, double brightness)
    {
        // Vérification pour la couleur noire : Luminosité faible et faible saturation (ajusté pour inclure gris foncés)
        if (brightness < 0.4 && saturation < 0.3)
            return "Black";

        // Détection du rouge : du rouge clair au rouge foncé, selon la saturation et la luminosité
        if (saturation > 0.3 && brightness > 0.1 && 
            ((hue >= 0 && hue <= 15) || (hue >= 15 && hue <= 30) || (hue >= 330 && hue <= 360))) // Plage de rouge clair à foncé
            return "Red";

        // Détection du bleu : du bleu clair au bleu foncé, selon la saturation et la luminosité
        if (saturation > 0.3 && brightness > 0.2 && 
            ((hue >= 190 && hue <= 220) || (hue >= 220 && hue <= 260))) // Plage de bleu clair à foncé
            return "Blue";

        // Couleur inconnue
        return "Unknown";
    }







}