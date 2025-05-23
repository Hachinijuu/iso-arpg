using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    //public GoX_Class playerClass;
    //public GoX_Body body;
    public int saveID;
    public string name;
    public CharacterPair character;
    public GoX_Difficulties difficulty;
    public Ability fusionAbility;
    
    // Need to know the ID of the identity skill to preload
    // What will a player profile keep track of?

    // The tutorials the player has passed through, and not show them if they've already done it
    // The players inventory and runes that they have

    // The player's difficulty
    public PlayerGameData gameData;
    public InventoryData inventoryData;
    
    // The player's stats
    // All stats should be known via the base class + the player's inventory and equipped gear

    public static PlayerProfile LoadPlayerProfile(string jsonData)
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
    public SavedRuneData[] items;

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

    public virtual void GetDataFromItem(ItemData item)
    {
        ItemID = item.ITEM_ID;
        rarity = item.rarity;
    }
    public SavedItemData LoadItemData(string jsonData)
    {
        return JsonUtility.FromJson<SavedRuneData>(jsonData);
    }

    public string SaveItemToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class SavedRuneData : SavedItemData
{
    public MainStat[] mainStats;
    public TrackedStat[] trackedStats;
    public SubStat[] subStats;
    public int itemSlot;
    // public override void GetDataFromItem(ItemData item)
    // {
    //     base.GetDataFromItem(item);
    //     RuneData rune = item as RuneData;
    //     mainStats = rune.mainStat;
    //     trackedStats = rune.trackedStat;
    //     subStats = rune.subStat;
    // }

    public virtual void GetDataFromItem(ItemData item, int slotIndex = -1)
    {
        base.GetDataFromItem(item);
        itemSlot = slotIndex;
        RuneData rune = item as RuneData;
        mainStats = rune.mainStat;
        trackedStats = rune.trackedStat;
        subStats = rune.subStat;
    }

    public SavedRuneData LoadRuneData(string jsonData)
    {
        return JsonUtility.FromJson<SavedRuneData>(jsonData);
    }
    public string SaveRuneToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}