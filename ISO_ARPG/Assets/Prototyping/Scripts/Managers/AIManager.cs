using UnityEngine;

public class AIManager : MonoBehaviour 
{
    private static AIManager instance = null;
    public static AIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AIManager>();
            if (!instance)
                Debug.LogWarning("[AIManager]: No AI manager found");
            return instance;
        }
    }

    // AI Manager gets the information from the level manager, get a reference to the level manager, can also use the instance
    LevelManager levelManager;

    #region UNITY FUNCTIONS
    void Start()
    {
        levelManager = LevelManager.Instance;
    }

    // level manager will send out data to all the enemies within the level
    // get rid of the object pool manager and have this level manager builds the pools accordingly
    #endregion
}