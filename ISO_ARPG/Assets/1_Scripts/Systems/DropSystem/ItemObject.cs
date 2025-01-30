using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "sykcorSystems/Items", order = 1)]
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
