using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneInteract : InteractableObject
{
    [SerializeField] Item item;
    protected override void InteractAction()
    {
        // When this is interacted with, give the player the item
        if (item != null && item.ItemData != null)
            Inventory.Instance.AddItem(item.ItemData);
        Destroy(gameObject);
    }
}
