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

    public void Awake()
    {
        saveCounter = 0;

        // Load the characters
        LoadProfiles();

        if (playerSaves != null && playerSaves.Count > 0)
        { 
            saveCounter = playerSaves.Count;
        }
    }
    [SerializeField] public List<PlayerProfile> playerSaves = new List<PlayerProfile>();
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

    public List<IdentityAbility> fusionAbilities;
    public RuneTable runes;

    int saveCounter;
    public void LoadProfiles()
    {
        // Loads all the files at the filepath
        // Get each folder in and load it to a profile

        string savePath = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string[] saves = Directory.GetFiles(savePath, "*.dat");

        foreach (string saveFile in saves)
        {
            PlayerProfile loadedProfile = new PlayerProfile();
            string saveData = File.ReadAllText(saveFile);
            // StreamReader readStream = new StreamReader(saveFile);
            // //StreamWriter readStream = new StreamWriter(saveFile);
            // string saveData = readStream.;
            loadedProfile = PlayerProfile.LoadPlayerProfile(saveData);
            if (loadedProfile.character.Character == null)
            {
                loadedProfile.character.Character = PlayerManager.Instance.GetControllerFromClassBody(loadedProfile.character.Guardian, loadedProfile.character.Body);
            }

            //readStream.Close();
            playerSaves.Add(loadedProfile);
        }

        Debug.Log("[SaveSystem] Loaded: " + playerSaves.Count + " save file(s)");

        //foreach (PlayerProfile profile in playerSaves)
        //{
        //    string loadFile = profile.LoadPlayerProfile();    // Save each file to it's own path and data 
        //}

        //foreach (string filePath in )
    }

    public Ability GetFusion(PlayerProfile profile)
    {
        Ability fusion = null;
        foreach (Ability ab in fusionAbilities)
        {
            if (ab == profile.fusionAbility)
            {
                fusion = ab;
                break;
            }
        }
        return fusion;
    }

    public PlayerProfile currentProfile;

    public void SaveProfile()
    {
        // Saves the current profile to files
        // Get the updated inventory data


        string savePath = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        if (Inventory.Instance != null)
        { 
            UpdateInventoryData();
        }


        // Instead of saving a new slot, use an existing slot if ID exists
        // Each save is it's own files

        string saveFile = currentProfile.SavePlayerProfile();
        string saveID = savePath + "Save" + currentProfile.saveID.ToString() + ".dat";
        File.WriteAllText(saveID, saveFile);
        //StreamWriter saveStream = new StreamWriter(saveID);
        //saveStream.WriteLine(saveFile);
        //saveStream.Close();
        Debug.Log("Saved: " + saveFile + "\n To: " + savePath);
    }

    public RuneData GetTemplateByID(int id)
    {
        RuneData rune = null;
        foreach (DropTableModifier runeDrop in runes.runes)
        {
            if (runeDrop.item.ITEM_ID == id)
            {
                rune = runeDrop.item as RuneData;
                return rune;
            }
        }
        return rune;
    }

    public void UpdateInventoryData()
    {
        InventoryData data = currentProfile.inventoryData;

        Dictionary<RuneData, int> equippedRunes = Inventory.Instance.EquippedItems;

        // Saving the inventory items
        List<SavedRuneData> savedItems = new List<SavedRuneData>();

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

        foreach (RuneData item in Inventory.Instance.Items)
        {
            SavedRuneData savedRune = new SavedRuneData();
            savedRune.GetDataFromItem(item, -1);
            savedItems.Add(savedRune);
            // else
            // {   
            //     SavedItemData savedItem = new SavedItemData();
            //     savedItem.GetDataFromItem(item);
            //     savedItems.Add(savedItem);
            // }
        }

        // Once the list is created, add it to the array
        data.items = savedItems.ToArray();


        data.dustAmount = Inventory.Instance.RuneDust;
        data.woodAmount = Inventory.Instance.WoodAmount;
        data.stoneAmount = Inventory.Instance.StoneAmount;
    }

    public void LoadProfile(string saveData)
    {
        // Loads the current profile into game data
        currentProfile = PlayerProfile.LoadPlayerProfile(saveData);
    }

    public void CreateNewProfile(string name = "test")
    {
        Debug.LogWarning("Created a new profile");
        currentProfile = new PlayerProfile();
        currentProfile.name = name;
        currentProfile.saveID = saveCounter;
        currentProfile.difficulty = GameManager.Instance.currDifficulty.difficulty;
        currentProfile.character = PlayerManager.Instance.currentCharacter;
        currentProfile.gameData = new PlayerGameData();
        currentProfile.inventoryData = new InventoryData();
        saveCounter++;
        SaveProfile();
        playerSaves.Add(currentProfile);
    }


    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveProfile();
        }
    }
}
