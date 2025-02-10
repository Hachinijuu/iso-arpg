using UnityEngine;
using UnityEngine.UI;

public enum ItemTypes { RESOURCE, GEAR, CONSUMMABLE };

public struct ItemEventArgs
{
    public ItemObject data;
    public ItemEventArgs(ItemObject itemData)
    {
        data = itemData;
    }
}

[CreateAssetMenu(fileName = "Item", menuName = "sykcorSystems/Items", order = 1)]
public class ItemObject : ScriptableObject
{
    #region VARIABLES
    #region Item Details
    [Header("Descriptions")]
    public string itemName;
    public string itemDescription;
    public Image itemIcon;
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
}
