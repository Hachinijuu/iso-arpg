using System.Collections.Generic;
using UnityEngine;
public class PublicEventManager : MonoBehaviour
{
    private static PublicEventManager instance;
    public static PublicEventManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PublicEventManager>();

            if (!instance)
                Debug.LogError("[PlayerManager]: No Public Event Manager exists!");

            return instance;
        }
    }

    public delegate void Cannot();
    public event Cannot onCannot;
    public void FireOnCannot() { if (onCannot != null) { onCannot(); } }

}
