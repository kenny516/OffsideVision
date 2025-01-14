using OffsideVision.model;

namespace OffsideVision.services;

public class CircleAnalyzer
{
    // function for getting the closest circle to the reference circle
    public static Circle GetectCircleClosest(List<Circle> circles, Circle circleRef)
    {
        Circle circleClosestBall = null;
        int distanceX = int.MaxValue;
        int distanceY = int.MaxValue;
        for (int i = 0; i < circles.Count; i++)
        {
            if (circles[i] == circleRef)
            {
                continue;
            }

            int newDistanceX = Math.Abs(circleRef.X - circles[i].X);
            int newDistanceY = Math.Abs(circleRef.Y - circles[i].Y);
            if (newDistanceX < distanceX && newDistanceY < distanceY)
            {
                distanceX = newDistanceX;
                distanceY = newDistanceY;
                circleClosestBall = circles[i];
            }
            else if (newDistanceX == distanceX && newDistanceY < distanceY)
            {
                distanceY = newDistanceY;
                circleClosestBall = circles[i];
            }
            else if (newDistanceX < distanceX && newDistanceY == distanceY)
            {
                distanceX = newDistanceX;
                circleClosestBall = circles[i];
            }
        }

        return circleClosestBall;
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

    // function for getting the same team by color
    public static List<Circle> GetSameTeam(List<Circle> circles, Circle circlePlayer)
    {
        return circles.FindAll(c => c.Color == circlePlayer.Color);
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
        var Defenseurs = GetectCircleClosest(circles, goalKeeper);
        return Defenseurs;
    }

    // Offside function 
    // sens = -1 for up
    // sens = 1 for down
    public static List<Circle> GetOffsideCircles(List<Circle> circles, Circle lastDefenseur, int sens)
    {
        List<Circle> offsidePlayer = new List<Circle>();
        for (int i = 0; i < circles.Count; i++)
        {
            if (lastDefenseur.Y - lastDefenseur.Radius > circles[i].Y - circles[i].Radius && sens > 0)
            {
                offsidePlayer.Add(circles[i]);
            }
            else if (lastDefenseur.Y + lastDefenseur.Radius < circles[i].Y + circles[i].Radius && sens < 0)
            {
                offsidePlayer.Add(circles[i]);
            }
        }

        return offsidePlayer;
    }
}