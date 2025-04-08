using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        string savePath = Application.persistentDataPath + "/saves";
        foreach (PlayerProfile profile in playerSaves)
        {
            string saveFile = profile.SavePlayerProfile();    // Save each file to it's own path and data 
        }

        // do a string write containing the profile data
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
        // Get the updated inventory data


        string savePath = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        UpdateInventoryData();

        string saveFile = currentProfile.SavePlayerProfile();
        //
        StreamWriter saveStream = new StreamWriter(savePath + "file.txt", true);
        saveStream.WriteLine(saveFile);
        saveStream.Close();
        Debug.Log("Saved: " + saveFile + "\n To: " + savePath);
    }

    public void UpdateInventoryData()
    {
        InventoryData data = currentProfile.inventoryData;

        Dictionary<RuneData, int> equippedRunes = Inventory.Instance.EquippedItems;

        // Saving the inventory items
        List<SavedItemData> savedItems = new List<SavedItemData>();
        foreach (ItemData item in Inventory.Instance.Items)
        {
            SavedItemData savedItem = new SavedItemData();
            savedItem.GetDataFromItem(item);
            savedItems.Add(savedItem);
        }

        // Once the list is created, add it to the array
        data.items = savedItems.ToArray();

        // Saving the equipped runes
        List<SavedRuneData> savedEquippedRunes = new List<SavedRuneData>();
        foreach (KeyValuePair<RuneData, int> equip in equippedRunes)
        {
            SavedRuneData savedRune = new SavedRuneData();
            savedRune.GetDataFromItem(equip.Key, equip.Value);
            savedEquippedRunes.Add(savedRune);
        }

        // Once the list is created, add it to the array
        data.equippedRunes = savedEquippedRunes.ToArray();

        data.dustAmount = Inventory.Instance.RuneDust;
        data.woodAmount = Inventory.Instance.WoodAmount;
        data.stoneAmount = Inventory.Instance.StoneAmount;
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


    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveProfile();
        }
    }
}
