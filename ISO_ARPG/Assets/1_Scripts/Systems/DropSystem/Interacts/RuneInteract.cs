using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneInteract : InteractableObject
{
    Item item;
    protected override void InteractAction()
    {
        // When this is interacted with, give the player the item
        Inventory.Instance.AddItem(item.ItemData);
    }
}
