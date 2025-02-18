using UnityEngine;
public class RuneSlot : ItemSlot
{
    // Unique slots, intended for stat applications to happen when bound to the character

    private PlayerStats stats;
    public void Start()
    {
        //if (Inventory.Instance != null)
        //{
        //    Inventory.Instance.OnItemReleased += context => { ApplyRuneEffects(); };
        //}
    }

    public void OnEnable()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();
    }

    // When should the RuneSlots apply stats to the player.
    // When the slot is not empty.

    // How can I tell if the player has dropped an item into this slot?
    // Add an event listener to the inventory system

    public void ApplyRuneEffects()
    {
        if (HasItem)    // only apply effects if a rune exists in this slot
        {
            Debug.Log("[Gear]: Applied rune effects");
        }
    }
}
