using UnityEngine;
using UnityEngine.UI;

public enum ItemRarity { COMMON, UNCOMMON, RARE, EPIC, RELIC }
public enum ItemTypes { RESOURCE, GEAR, RUNE, CONSUMMABLE };

public struct ItemEventArgs
{
    public ItemData data;
    public ItemEventArgs(ItemData itemData)
    {
        data = itemData;
    }
}

[CreateAssetMenu(fileName = "Item", menuName = "sykcorSystems/Drops/Item", order = 2)]
public class ItemData : ScriptableObject
{
    #region VARIABLES
    #region Item Details
    [Header("Descriptions")]
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public ItemRarity rarity;
    public ItemTypes type;
    #endregion
    public GameObject prefab;
    public float dropChance;
    public bool showPopup;
    #endregion
    #region EVENTS
    public delegate void ItemAcquired(ItemEventArgs e);
    public event ItemAcquired OnItemAcquired;
    public void FireAcquired(ItemEventArgs e) { if (OnItemAcquired != null) { OnItemAcquired(e); } }
    #endregion

    public void LoadSpriteToImage(Image loadTo)
    {
        loadTo.sprite = itemIcon;
    }
}

// What are the possible types of runes that can be derived
// Stat increase rune --> these will get a reference to the player data and apply the stat bonuses themselves?
// Other rune types tbd
