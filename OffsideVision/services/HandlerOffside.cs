using System.Drawing;
using OffsideVision.model;

namespace OffsideVision.services;

public class HandlerOffside
{
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
                if (Utils.IsCloseColor(pixelColor, Color.Red) ||
                    Utils.IsCloseColor(pixelColor, Color.Blue) ||
                    Utils.IsCloseColor(pixelColor, Color.Black))
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
        double radius = cluster.Average(p => Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2)));

        // Vérifier si le rayon est dans les limites spécifiées
        if (radius >= minRadius && radius <= maxRadius)
        {
            return new Circle
            {
                X = centerX,
                Y = centerY,
                Radius = (int)radius,
                Color = Utils.IsCloseColor(color, Color.Red) ? "Red" :
                    Utils.IsCloseColor(color, Color.Blue) ? "Blue" : "black"
            };
        }

        return null;
    }

    public static void CheckOffside(List<Circle> circles)
    {
        Circle ball = circles.FirstOrDefault(c => c.Color == "black");
        if (ball == null)
        {
            Console.WriteLine("Pas de joueur avec le ballon trouvé !");
            return;
        }

        // Séparer les cercles par couleur d'équipe
        var redTeam = circles.Where(c => c.Color == "Red").ToList();
        var blueTeam = circles.Where(c => c.Color == "Blue").ToList();

        // Déterminer l'équipe en possession du ballon
        List<Circle> attackingTeam = ball.Color == "Red" ? blueTeam : redTeam;
        List<Circle> defendingTeam = ball.Color == "Red" ? redTeam : blueTeam;

        // Calculer la ligne des défenseurs (l'avant-dernier défenseur)
        var defendingTeamSorted = defendingTeam.OrderBy(c => c.X).ToList();
        int lastDefenderX = defendingTeamSorted.Last().X;  // Le défenseur le plus proche de la ligne de but

        // Vérifier les joueurs de l'équipe attaquante pour savoir s'ils sont hors-jeu
        foreach (var player in attackingTeam)
        {
            if (player.X < lastDefenderX)
            {
                Console.WriteLine($"Le joueur à la position X = {player.X}, Y = {player.Y} est HORS-JEU !");
            }
            else
            {
                Console.WriteLine($"Le joueur à la position X = {player.X}, Y = {player.Y} est EN POSITION VALIDE.");
            }
        }
    }

    public static Bitmap AnnotateImage(Bitmap image, List<Circle> circles, List<Circle> offsidePlayers)
    {
        Bitmap annotatedImage = new Bitmap(image);
        using (Graphics g = Graphics.FromImage(annotatedImage))
        {
            Pen pen = new Pen(Color.Red, 2);
            Font font = new Font("Arial", 12);
            Brush brushHJ = Brushes.Azure;
            Brush brushER = Brushes.Green;

            foreach (var circle in circles)
            {
                // Vérifier si le joueur est hors-jeu
                bool isOffside = offsidePlayers.Contains(circle);

                // Dessiner un cercle
                g.DrawEllipse(pen, circle.X - 10, circle.Y - 10, 20, 20);

                // Ajouter l'étiquette (HJ ou ER)
                string label = isOffside ? "HJ8888888" : "ER";
                Brush brush = isOffside ? brushHJ : brushER;
                g.DrawString(label, font, brush, circle.X + 5, circle.Y - 20);
            }
        }

        return annotatedImage;
    }
    
}