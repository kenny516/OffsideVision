using System.Windows.Documents;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;
using OffsideVision.model;

namespace OffsideVision.services;

public class CircleAnalyzer
{
    // function for getting the closest circle to the reference circle
    public static Circle GetClosestCircle(List<Circle> circles, Circle circleRef)
    {
        if (circles == null || circles.Count == 0)
            throw new ArgumentException("La liste des cercles ne peut pas être vide.");

        Circle closestCircle = null;
        double minDistance = double.MaxValue;

        foreach (var circle in circles)
        {
            if (circle == circleRef && circle.Color == "Black")
                continue;

            // Calcul de la distance euclidienne entre les centres des cercles
            double distance = Math.Sqrt(Math.Pow(circle.X - circleRef.X, 2) +
                                        Math.Pow(circle.Y - circleRef.Y, 2));

            if (distance < minDistance)
            {
                minDistance = distance;
                closestCircle = circle;
            }
        }

        return closestCircle;
    }

    public static Circle GEtClosestCircleDiff(List<Circle> circles,Circle circleRef,Circle diff) 
    {
        if (circles == null || circles.Count == 0)
            throw new ArgumentException("La liste des cercles ne peut pas être vide.");

        Circle closestCircle = null;
        double minDistance = double.MaxValue;

        foreach (var circle in circles)
        {
            if (circle == circleRef || circle.Color == diff.Color)
                continue;

            // Calcul de la distance euclidienne entre les centres des cercles
            double distance = Math.Sqrt(Math.Pow(circle.X - circleRef.X, 2) +
                                        Math.Pow(circle.Y - circleRef.Y, 2));

            if (distance < minDistance)
            {
                minDistance = distance;
                closestCircle = circle;
            }
        }

        return closestCircle;
        
    }
    public static Circle Getball(List<Circle> circles)
    {
        return circles.FirstOrDefault(c => c.Color == "Black") ?? throw new Exception("No ball detected");
    }

    //function for getting the goalKeeper by color (Team)
    public static Circle GetectGoalKeeper(List<Circle> circles, string color)
    {
        int minY = Int32.MaxValue;
        Circle circleGoalKeeper = null;
        for (int i = 0; i < circles.Count; i++)
        {
            if (minY > circles[i].Y)
            {
                minY = circles[i].Y;
                circleGoalKeeper = circles[i];
            }
        }

        if (circleGoalKeeper != null && !circleGoalKeeper.Color.Equals(color))
        {
            int maxY = Int32.MinValue;
            circleGoalKeeper = null;
            for (int i = 0; i < circles.Count; i++)
            {
                if (maxY < circles[i].Y)
                {
                    maxY = circles[i].Y;
                    circleGoalKeeper = circles[i];
                }
            }
        }

        if (circleGoalKeeper == null)
        {
            throw new Exception("No Goal keeper detected");
        }

        return circleGoalKeeper;
    }

    public static Circle GetCarrier(List<Circle> circles)
    {
        var ball = Getball(circles);
        var carrier = GetClosestCircle(circles, ball);
        return carrier;
    }

    // function for getting the same team by color
    public static List<Circle> GetSameTeam(List<Circle> circles, Circle circlePlayer)
    {
        return circles.FindAll(c => c.Color == circlePlayer.Color);
    }
    public static List<Circle> GetOpposingTeam(List<Circle> circles, Circle circlePlayer)
    {
        return circles.FindAll(c => c.Color != circlePlayer.Color && c.Color != "Black");
    }

    public static List<Circle> GetMemberTeamBeforCarrier(List<Circle> circles, Circle carrier, Circle goalKeeper)
    {
        List<Circle> PlayerPossibleOffside = new List<Circle>();
        for (int i = 0; i < circles.Count; i++)
        {
            if (circles[i] != carrier && circles[i].Color == carrier.Color)
            {
                if (circles[i].Y < carrier.Y && carrier.Y - goalKeeper.Y < 0)
                {
                    PlayerPossibleOffside.Add(circles[i]);
                }
                else if (circles[i].Y > carrier.Y && carrier.Y - goalKeeper.Y > 0)
                {
                    PlayerPossibleOffside.Add(circles[i]);
                }
            }
        }

        return PlayerPossibleOffside;
    }

    // List of circle of the same team 
    public static Circle GetLastDefenseur(List<Circle> circles, Circle goalKeeper)
    {
        Circle circleClosestBall = null;
        double minDistance = int.MaxValue;
        foreach (var t in circles)
        {
            if (t == goalKeeper || t.Color != goalKeeper.Color)
            {
                continue;
            }

            double distance = Math.Sqrt(Math.Pow(t.X - goalKeeper.X, 2) +
                                        Math.Pow(t.Y - goalKeeper.Y, 2));

            if (distance < minDistance)
            {
                minDistance = distance;
                circleClosestBall = t;
            }
        }
        return circleClosestBall;
    }

    private static int GetSensAttaque(List<Circle> circles,Circle carrier)
    {
        var goalKeeper = GetectGoalKeeper(circles, carrier.Color);
        if (goalKeeper.Y - carrier.Y > 0)
        {
            return 1;
        }

        return -1;
    }

    // Offside function 
    // sens = -1 for up
    // sens = 1 for down
    public static List<Circle> GetOffsideCircles(ref int lineLastDefenseur,List<Circle> circles,int centreY)
    {
        var carrier = GetCarrier(circles);
        var goalKeeper = GetectGoalKeeper(circles,carrier.Color);
        
        
        var opTeam = GetOpposingTeam(circles,carrier);
        var opGoalKeeper = GetectGoalKeeper(circles,opTeam.First().Color);;
        var oplastDefenseur = GetLastDefenseur(circles,opGoalKeeper);
        var sens = GetSensAttaque(circles,carrier);
        
        
        var attackerPossibleOffside = GetMemberTeamBeforCarrier(circles, carrier, goalKeeper);
        
        var offsidePlayer = new List<Circle>();
        
        Console.WriteLine("sense : " + sens);
        Console.WriteLine("carrier : " + carrier.Color);
        Console.WriteLine("Attacker count : " + attackerPossibleOffside.Count);
        Console.WriteLine("goalKeeper : " + opGoalKeeper.Color);

        Console.WriteLine("last defenseur x "+oplastDefenseur.X+" y:"+oplastDefenseur.Y+" color"+oplastDefenseur.Color);
        
        foreach (var attacker in attackerPossibleOffside)
        {
            Console.WriteLine("Attacker x "+attacker.X+" y:"+attacker.Y);
            if (oplastDefenseur.Y - oplastDefenseur.Radius > attacker.Y - attacker.Radius && sens > 0)
            {
                lineLastDefenseur = oplastDefenseur.Y - oplastDefenseur.Radius;
                if (attacker.Y -attacker.Radius > centreY)
                {
                    continue;
                }
                offsidePlayer.Add(attacker);
            }
            else if (oplastDefenseur.Y + oplastDefenseur.Radius < attacker.Y + attacker.Radius && sens < 0)
            {
                lineLastDefenseur = oplastDefenseur.Y + oplastDefenseur.Radius;
                if (attacker.Y +attacker.Radius < centreY)
                {
                    continue;
                }
                offsidePlayer.Add(attacker);
            }
        }
        Console.WriteLine("offside count : " + offsidePlayer.Count);

        return offsidePlayer;
    }

    public static List<Circle> GetOffsideTeam(List<Circle> circles,Circle carrier,int centreY)
    {
        var goalKeeper = GetectGoalKeeper(circles,carrier.Color);
        
        
        var opTeam = GetOpposingTeam(circles,carrier);
        var opGoalKeeper = GetectGoalKeeper(circles,opTeam.First().Color);;
        var oplastDefenseur = GetLastDefenseur(circles,opGoalKeeper);
        var sens = GetSensAttaque(circles,carrier);
        
        
        var attackerPossibleOffside = GetMemberTeamBeforCarrier(circles, carrier, goalKeeper);
        
        var offsidePlayer = new List<Circle>();
        
        Console.WriteLine("sense : " + sens);
        Console.WriteLine("carrier : " + carrier.Color);
        Console.WriteLine("Attacker count : " + attackerPossibleOffside.Count);
        Console.WriteLine("goalKeeper color : " + opGoalKeeper.Color);

        Console.WriteLine("last defenseur x "+oplastDefenseur.X+" y:"+oplastDefenseur.Y+" color"+oplastDefenseur.Color);
        
        foreach (var attacker in attackerPossibleOffside)
        {
            Console.WriteLine("Attacker x "+attacker.X+" y:"+attacker.Y);
            if (oplastDefenseur.Y - oplastDefenseur.Radius > attacker.Y - attacker.Radius && sens > 0)
            {
                if (attacker.Y -attacker.Radius < centreY)
                {
                    continue;
                }
                offsidePlayer.Add(attacker);
            }
            else if (oplastDefenseur.Y + oplastDefenseur.Radius < attacker.Y + attacker.Radius && sens < 0)
            {
                if (attacker.Y +attacker.Radius > centreY)
                {
                    continue;
                }
                offsidePlayer.Add(attacker);
            }
        }
        Console.WriteLine("offside count : " + offsidePlayer.Count);

        return offsidePlayer;   
    }
}