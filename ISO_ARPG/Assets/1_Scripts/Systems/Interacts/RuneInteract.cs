using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneInteract : InteractableObject
{
    [SerializeField] Item item;
    [SerializeField] GameObject relicFX;
    [SerializeField] GameObject epicFX;

    public void OnEnable()
    {
        Debug.Log("[RuneInteract]: I activated, I will now set a rune colour: " + item.ItemData.rarity);
        if (item.ItemData.rarity == ItemRarity.EPIC)
        {
            epicFX.SetActive(true);
        }
        else if (item.ItemData.rarity == ItemRarity.RELIC)
        {
            relicFX.SetActive(true);
        }
    }

    protected override void InteractAction()
    {
        // When this is interacted with, give the player the item
        if (item != null && item.ItemData != null)
            Inventory.Instance.AddItem(item.ItemData);
        Destroy(gameObject);
    }
}
