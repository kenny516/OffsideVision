using System.Drawing;
using OffsideVision.model;
using OpenCvSharp;
using Point = OpenCvSharp.Point;  // Utiliser OpenCvSharp.Point au lieu de System.Drawing.Point

namespace OffsideVision.services
{
    public class HandlerOffside
    {
        // Détection des cercles pour une couleur donnée
        private static List<Circle> DetectCerclesByColor(Bitmap image, Scalar lowerBound, Scalar upperBound, Color color)
        {
            List<Circle> Cercles = new List<Circle>();

            // Convertir Bitmap en Mat (OpenCvSharp)
            Mat matImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

            // Convertir l'image en HSV (Teinte, Saturation, Valeur)
            Mat hsvImage = new Mat();
            Cv2.CvtColor(matImage, hsvImage, ColorConversionCodes.BGR2HSV);

            // Filtrer l'image HSV pour n'obtenir que la couleur souhaitée
            Mat mask = new Mat();
            Cv2.InRange(hsvImage, lowerBound, upperBound, mask);

            // Trouver les contours dans le masque
            OpenCvSharp.Point[][] contours; // Utiliser OpenCvSharp.Point[][] au lieu de System.Drawing.Point[][]
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(mask, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // Parcourir les contours pour déterminer les positions des cercles
            foreach (var contour in contours)
            {
                // Calculer le cercle englobant pour chaque contour
                Point2f center;
                float radius;
                Cv2.MinEnclosingCircle(contour, out center, out radius);

                if (radius > 5) // Ignorer les petits bruits
                {
                    Cercles.Add(new Circle((int)center.X, (int)center.Y, (int)radius, color.Name));
                }
            }

            return Cercles;
        }

        // Détection des cercles pour toutes les couleurs possibles (bleu, rouge, noir)
        public static List<Circle> CircleDetect(Bitmap image, int minRadius, int maxRadius)
        {
            // Définir les plages de couleurs HSV pour les joueurs en bleu et en rouge
            Scalar blueLower = new Scalar(100, 150, 0); 
            Scalar blueUpper = new Scalar(140, 255, 255);
            Scalar redLower1 = new Scalar(0, 150, 0);
            Scalar redUpper1 = new Scalar(10, 255, 255);
            Scalar redLower2 = new Scalar(170, 150, 0);
            Scalar redUpper2 = new Scalar(180, 255, 255);
            Scalar blackLower = new Scalar(0, 0, 0);
            Scalar blackUpper = new Scalar(180, 255, 50);

            // Détecter les cercles de chaque couleur
            List<Circle> blueCercles = DetectCerclesByColor(image, blueLower, blueUpper, Color.Blue);
            List<Circle> redCercles = DetectCerclesByColor(image, redLower1, redUpper1, Color.Red);
            List<Circle> blackCercles = DetectCerclesByColor(image, blackLower, blackUpper, Color.Black);

            // Ajouter les cercles détectés
            redCercles.AddRange(DetectCerclesByColor(image, redLower2, redUpper2, Color.Red));
            List<Circle> Cercles = new List<Circle>();
            Cercles.AddRange(blueCercles);
            Cercles.AddRange(redCercles);
            Cercles.AddRange(blackCercles);

            return Cercles;
        }

        // Annotation de l'image avec les cercles et l'état des joueurs (Hors-jeu ou en règle)
        public static Bitmap AnnotateImage(Bitmap image, List<Circle> circles, List<Circle> offsidePlayers)
        {
            Bitmap annotatedImage = new Bitmap(image);

            using Graphics g = Graphics.FromImage(annotatedImage);
            foreach (var circle in circles)
            {
                // Vérifier si le joueur est hors-jeu ou en règle
                var isOffside = offsidePlayers.Any(p => p.X == circle.X && p.Y == circle.Y && p.Radius == circle.Radius);
                    
                // Définir la couleur de l'étiquette en fonction de l'état
                var labelColor = isOffside ? Color.Red : Color.LightBlue;

                // Dessiner le cercle avec un bord jaune
                g.DrawEllipse(new Pen(Color.Yellow, 2),
                    circle.X - circle.Radius,
                    circle.Y - circle.Radius,
                    circle.Radius * 2,
                    circle.Radius * 2);

                // Ajouter l'étiquette "HJ" pour Hors-Jeu ou "ER" pour En-Règle
                var label = isOffside ? "HJ" : "";
                using Brush brush = new SolidBrush(labelColor);
                g.DrawString(label, new Font("Arial", 12, FontStyle.Bold), brush,
                    circle.X + circle.Radius,
                    circle.Y - circle.Radius);
            }

            return annotatedImage;
        }

        public static void DrawLine(Bitmap image, int y)
        {
            // Vérification pour s'assurer que 'y' est dans les limites de l'image
            if (y < 0 || y >= image.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y),
                    "La valeur de y doit être comprise entre 0 et la hauteur de l'image.");
            }

            using var graphics = Graphics.FromImage(image);
            // Définir une couleur et une épaisseur de pinceau
            using var pen = new Pen(Color.Red, 2);
            // Dessiner une ligne horizontale sur l'image
            graphics.DrawLine(pen, 0, y, image.Width - 1, y);
        }
    }
}
