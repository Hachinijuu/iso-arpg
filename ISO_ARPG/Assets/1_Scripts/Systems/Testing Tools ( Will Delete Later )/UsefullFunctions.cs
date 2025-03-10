using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefullFunctions
{

    public static void DebugRay(Vector3 origin, Vector3 v, Color c)
    {
        Debug.DrawRay(origin, v * v.magnitude, c);
    }


    public static Vector3 ClampMagnitude(Vector3 v, float max)
    {

        if (v.magnitude > max)
            return v.normalized * max;
        else
            return v;

    }
}
