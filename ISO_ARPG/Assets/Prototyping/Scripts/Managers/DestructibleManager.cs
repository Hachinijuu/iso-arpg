using System.Collections.Generic;
using UnityEngine;

public class DestructibleManager : MonoBehaviour 
{
    private static DestructibleManager instance = null;
    public static DestructibleManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DestructibleManager>();
            if (!instance)
                Debug.LogWarning("[DestructibleManager] No Destructible manager found");
            return instance;
        }
    }


    public List<ObjectPool> Destructibles;  

    #region SCENE LOADS
    // When a level (scene) is loading, call this function
    public void LevelLoading()
    {

    }

    // When a level (scene) is unloading, call this function
    public void LevelUnloading()
    {

    }
#endregion  
}