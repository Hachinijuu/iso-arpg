using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    //public GoX_Class playerClass;
    //public GoX_Body body;
    public CharacterPair character;
    public GoX_Difficulties difficulty;
    
    // Need to know the ID of the identity skill to preload
    // What will a player profile keep track of?

    // The tutorials the player has passed through, and not show them if they've already done it
    // The players inventory and runes that they have

    // The player's difficulty
    public PlayerGameData gameData;
    public InventoryData inventoryData;
    
    // The player's stats
    // All stats should be known via the base class + the player's inventory and equipped gear

    public PlayerProfile LoadPlayerProfile(string jsonData)
    {
        return JsonUtility.FromJson<PlayerProfile>(jsonData);
    }

    public string SavePlayerProfile()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class PlayerGameData
{
    // This is the data relative to the game, which tutorials are completed
    public bool seenCutscene;
    public bool seenRune;
    public bool seenHendok;
    public bool seenIdentity;

    public PlayerGameData LoadGameData(string jsonData)
    {
        return JsonUtility.FromJson<PlayerGameData>(jsonData);
    }

    public string SaveGameDataToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class InventoryData
{
    // How to pack the item data into this class?
    public int dustAmount;
    public int woodAmount;
    public int stoneAmount;
    public SavedRuneData[] equippedRunes;
    public SavedItemData[] items;

    public InventoryData LoadInventory(string jsonData)
    {
        return JsonUtility.FromJson<InventoryData>(jsonData);
    }
    public string SaveInventoryToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class SavedItemData
{
    // This contains the data of a single item, inventory will contain an array of items
    // Save the ID, for remapping of scriptable objects
    // Save the unique information of the RUNE / ITEM
    public int ItemID;
    public ItemRarity rarity;
    public SavedItemData LoadItemData(string jsonData)
    {
        return JsonUtility.FromJson<SavedRuneData>(jsonData);
    }

    public string SaveItemToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

public class SavedRuneData : SavedItemData
{
    public GearSlotType gearSlot;
    public RuneType runeType;
    public MainStat[] mainStats;
    public TrackedStat[] trackedStats;
    public SubStat[] subStats;

    public SavedRuneData LoadRuneData(string jsonData)
    {
        return JsonUtility.FromJson<SavedRuneData>(jsonData);
    }
    public string SaveRuneToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}