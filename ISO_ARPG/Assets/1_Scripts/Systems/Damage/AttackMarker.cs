using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{
    public static float hitResetTime = 0.25f;
    public void OnEnable()
    {
        Destroy(this, hitResetTime);
    }
}
