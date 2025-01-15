namespace OffsideVision.model;

public class Circle
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public int Radius { get; set; }
    public string Color { get; set; }
    
    public Circle(int x, int y, int radius, string color)
    {
        X = x;
        Y = y;
        Radius = radius;
        Color = color ?? "defaultColor";  // Valeur par défaut si null
    }

    public Circle()
    {
       
    }
}
