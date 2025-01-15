using System.Drawing;
using OffsideVision.model;

namespace OffsideVision.services;

public class HandlerOffside
{
    // static Color Team1Color = Color.FromArgb(250, 105, 105);
    // static Color Team2Color = Color.FromArgb(86, 162, 232);
    // static Color ballColor = Color.FromArgb(18, 18, 18);
    static Color Team1Color = Color.Red;
    static Color Team2Color = Color.FromArgb(21, 120, 214);
    static Color ballColor = Color.Black;

    public static List<Circle> CircleDetect(Bitmap image, int minRadius, int maxRadius)
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
                    // Trouver un cluster de pixels similaires
                    var cluster = FloodFill(image, x, y, pixelColor, visited);

                    // Vérifier si le cluster forme un cercle avec un rayon valide
                    Circle? circle = GetCircleFromCluster(cluster, minRadius, maxRadius, pixelColor);

                    if (circle != null)
                    {
                        circles.Add(circle);
                    }
                }
            }
        }

        return circles;
    }

    private static List<Point> FloodFill(Bitmap image, int startX, int startY, Color targetColor, bool[,] visited)
    {
        List<Point> cluster = new List<Point>();
        Queue<Point> queue = new Queue<Point>();
        queue.Enqueue(new Point(startX, startY));

        while (queue.Count > 0)
        {
            Point point = queue.Dequeue();

            if (point.X < 0 || point.X >= image.Width || point.Y < 0 || point.Y >= image.Height)
                continue;

            if (visited[point.X, point.Y])
                continue;

            Color currentColor = image.GetPixel(point.X, point.Y);
            if (!Utils.IsCloseColor(currentColor, targetColor))
                continue;

            // Ajouter au cluster
            cluster.Add(point);
            visited[point.X, point.Y] = true;

            // Ajouter les voisins
            queue.Enqueue(new Point(point.X + 1, point.Y));
            queue.Enqueue(new Point(point.X - 1, point.Y));
            queue.Enqueue(new Point(point.X, point.Y + 1));
            queue.Enqueue(new Point(point.X, point.Y - 1));
        }

        return cluster;
    }

    private static Circle? GetCircleFromCluster(List<Point> cluster, int minRadius, int maxRadius, Color color)
    {
        // Calculer les limites du cluster
        int minX = cluster.Min(p => p.X);
        int maxX = cluster.Max(p => p.X);
        int minY = cluster.Min(p => p.Y);
        int maxY = cluster.Max(p => p.Y);

        // Calculer le centre
        int centerX = (minX + maxX) / 2;
        int centerY = (minY + maxY) / 2;

        // Calculer le rayon approximatif
        double radius = cluster.Max(p => Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2)));

        // Vérifier si le rayon est dans les limites spécifiées
        if (radius >= minRadius && radius <= maxRadius)
        {
            return new Circle
            {
                X = centerX,
                Y = centerY,
                Radius = (int)radius,
                Color = Utils.IsCloseColor(color, Team1Color) ? "Red" :
                    Utils.IsCloseColor(color, Team2Color) ? "Blue" : "Black"
            };
        }

        return null;
    }


    public static Bitmap AnnotateImage(Bitmap image, List<Circle> circles, List<Circle> offsidePlayers)
    {
        Bitmap annotatedImage = new Bitmap(image);
        using (Graphics g = Graphics.FromImage(annotatedImage))
        {
            Pen pen = new Pen(Color.Khaki, 3);
            Font font = new Font("Arial", 15);
            Brush brushHJ = Brushes.Crimson;
            Brush brushER = Brushes.Aqua;

            foreach (var circle in circles)
            {
                // Vérifier si le joueur est hors-jeu
                bool isOffside = offsidePlayers.Contains(circle);

                // Dessiner un cercle
                g.DrawEllipse(pen, circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2,
                    circle.Radius * 2);

                // Ajouter l'étiquette (HJ ou ER)
                string label = isOffside ? "HJ" : "ER";
                Brush brush = isOffside ? brushHJ : brushER;
                g.DrawString(label, font, brush, circle.X + circle.Radius, circle.Y - circle.Radius);
            }
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

        using (Graphics graphics = Graphics.FromImage(image))
        {
            // Définir une couleur et une épaisseur de pinceau
            using (Pen pen = new Pen(Color.Red, 2)) // Ligne rouge avec une épaisseur de 2
            {
                // Dessiner une ligne horizontale sur l'image
                graphics.DrawLine(pen, 0, y, image.Width - 1, y);
            }
        }
    }
}