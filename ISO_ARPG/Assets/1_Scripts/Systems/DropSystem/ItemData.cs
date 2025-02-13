using UnityEngine;
using UnityEngine.UI;

public enum ItemTypes { RESOURCE, GEAR, CONSUMMABLE };

public struct ItemEventArgs
{
    public ItemData data;
    public ItemEventArgs(ItemData itemData)
    {
        data = itemData;
    }
}

[CreateAssetMenu(fileName = "Item", menuName = "sykcorSystems/Items", order = 1)]
public class ItemData : ScriptableObject
{
    #region VARIABLES
    #region Item Details
    [Header("Descriptions")]
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
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
