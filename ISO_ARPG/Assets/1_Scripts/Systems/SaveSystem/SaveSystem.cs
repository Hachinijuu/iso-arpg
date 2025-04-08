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
    public string[] saveFiles;

    public void SaveProfiles()
    {
        // Saves all the loaded profiles to the filepath
        foreach (PlayerProfile profile in playerSaves)
        {
            string saveFile = profile.SavePlayerProfile();    // Save each file to it's own path and data 
        }
    }
    
    public void LoadProfiles()
    {
        // Loads all the files at the filepath
        // Get each folder in and load it to a profile
        //foreach (string filePath in )
    }

    public PlayerProfile currentProfile;

    public void SaveProfile()
    {
        // Saves the current profile to files
        string saveFile = currentProfile.SavePlayerProfile();
    }

    public void LoadProfile(string saveData)
    {
        // Loads the current profile into game data
        currentProfile = currentProfile.LoadPlayerProfile(saveData);
    }

    public void CreateNewProfile()
    {
        Debug.LogWarning("Created a new profile");
        currentProfile = new PlayerProfile();
        currentProfile.difficulty = GameManager.Instance.currDifficulty.difficulty;
        currentProfile.character = PlayerManager.Instance.currentCharacter;
        currentProfile.gameData = new PlayerGameData();
        currentProfile.inventoryData = new InventoryData();
    }
}
