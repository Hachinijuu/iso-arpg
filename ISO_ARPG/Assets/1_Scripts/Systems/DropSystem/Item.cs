using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // There are two variations of items, auto-pickup on collisions, or click to pickup (interactable)
    // This will be assigned with a scriptableObject reference to the item
    // Pickup handling for other types will be handled in playerController
    // All items should be tagged interactable
    #region VARIABLES
    [SerializeField] ItemData itemData;

    #endregion
    #region UNITY FUNCTIONS
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        if (itemData.type == ItemTypes.RESOURCE)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // Set the object to false, return it to the supposed object pool it was pulled from.
                Inventory.Instance.AddItem(itemData);
                itemData.FireAcquired(new ItemEventArgs(itemData));
                Destroy(gameObject);
                //gameObject.SetActive(false);
                
            }
        }
    }
    #endregion

    #region FUNCTIONALITY
    public void LoadItemData(ItemData data)
    {
        itemData = data;
    }
    #endregion
}
