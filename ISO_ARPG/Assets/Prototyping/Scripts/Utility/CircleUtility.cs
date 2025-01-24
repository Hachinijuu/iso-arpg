using System.Collections.Generic;
using UnityEngine;

public class CircleUtility
{
    // CircleUtility builds a list of points that forms a circle
    // Since the points are precalcualated by this utility, any class that needs this functionality does not need to calculate the points.
    public const int MAX_CIRCLE_POSITIONS = 360;
    public const float FULL_ROTATION = 360.0f;
    public const int ANGLE_INC = (int)FULL_ROTATION / MAX_CIRCLE_POSITIONS;

    private static List<Vector3> circlePositions;
    public static List <Vector3> CircleListInstance
    {
        get
        {
            if (circlePositions == null)
            {
                circlePositions = new List<Vector3>();
                InitCirclePositions();
            }
            return circlePositions;
        }
    }

    private static void InitCirclePositions()
    {
        float angle = 0.0f;
        for (int i = 0; i < MAX_CIRCLE_POSITIONS; i++)
        {
            Vector3 posOnCircle;

            posOnCircle.x = Mathf.Cos(Mathf.Deg2Rad * angle);
            posOnCircle.z = Mathf.Sin(Mathf.Deg2Rad * angle);
            posOnCircle.y = 0;
            circlePositions.Add(posOnCircle);
            angle += ANGLE_INC;
        }
    }

}