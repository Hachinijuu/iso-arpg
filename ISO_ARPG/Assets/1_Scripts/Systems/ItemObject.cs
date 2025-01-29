using UnityEngine;
using UnityEngine.UI;

public class ItemObject : ScriptableObject
{
    #region VARIABLES
    #region Item Details
    [Header("Descriptions")]
    public string itemName;
    public string itemDescription;
    public Image itemIcon;
    #endregion

    public GameObject prefab;
    public float dropChance;
    #endregion
}
