using UnityEngine;

public class Item : MonoBehaviour
{
    // There are two variations of items, auto-pickup on collisions, or click to pickup (interactable)
    // This will be assigned with a scriptableObject reference to the item
    // Pickup handling for other types will be handled in playerController
    // All items should be tagged interactable
    #region VARIABLES
    public bool requiresClick;
    public float dropYOffset = 1.0f;

    public ItemData ItemData { get { return itemData; } }
    [SerializeField] ItemData itemData;

    #endregion
    #region UNITY FUNCTIONS

    public void HandlePlayerPickup(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            itemData.FireAcquired(new ItemEventArgs(itemData));
            // Since the player has collided, see what needs to be added to the inventory
            if (itemData is PotionData pd)
            {
                switch (pd.potionType)
                {
                    case PotionTypes.HEALTH:
                        if (Inventory.Instance.GetNumHealthPotions() < Inventory.Instance.maxHealthPotions)
                        {
                            Inventory.Instance.AddPotion(pd);
                            Destroy(gameObject);
                        }
                        break;
                    case PotionTypes.MANA:
                        if (Inventory.Instance.GetNumManaPotions() < Inventory.Instance.maxManaPotions)
                        {
                            Inventory.Instance.AddPotion(pd);
                            Destroy(gameObject);
                        }
                        break;
                }
                //Inventory.Instance.AddPotion(pd);
            }
            else if (itemData is ResourceData rd)
            {
                switch (rd.resourceType)
                {
                    case ResourceTypes.DUST:
                        Inventory.Instance.RuneDust += rd.amount;
                        break;
                    case ResourceTypes.WOOD:
                        Inventory.Instance.WoodAmount += rd.amount;
                        break;
                    case ResourceTypes.STONE:
                        Inventory.Instance.StoneAmount += rd.amount;
                        break;
                }
                Destroy(gameObject);
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (requiresClick) { return; }
        HandlePlayerPickup(other);
        //Debug.Log(other.gameObject.tag);
    }
    #endregion

    #region FUNCTIONALITY
    public void LoadItemData(ItemData data)
    {
        itemData = data;
    }
    #endregion
}
