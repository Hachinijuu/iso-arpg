using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChainHitMarker : MonoBehaviour
{
    public int chainID;
    public static float hitResetTime = 0.25f;
    public void OnEnable()
    {
        // Remove this script
        Destroy(this, hitResetTime);
    }
    
    public ChainHitMarker(int id)
    {
        chainID = id;
    }

    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        Handles.Label(transform.position, "ChainID: " + chainID.ToString(), style);
    }
}
