using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem instance;
    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SaveSystem>();

            if (!instance)
                Debug.LogError("[SaveSystem]: No Save System exists!");

            return instance;
        }
    }

    public PlayerProfile[] playerSaves;

    public void SaveProfiles()
    {
        // Saves all the loaded profiles to the filepath
    }
    
    public void LoadProfiles()
    {
        // Loads all the files at the filepath
    }

    PlayerProfile currentProfile;

    public void SaveProfile()
    {
        // Saves the current profile to files
    }

    public void LoadProfile()
    {
        // Loads the current profile into game data
    }
}
