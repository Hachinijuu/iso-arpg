using UnityEngine;
public class RuneSlot : ItemSlot
{
    // Unique slots, intended for stat applications to happen when bound to the character
    public RuneType type;
    private PlayerStats stats;
    public void Start()
    {
        //if (Inventory.Instance != null)
        //{
        //    Inventory.Instance.OnItemReleased += context => { ApplyRuneEffects(); };
        //}
    }

    // The current issue is that the inventory is not bound by character, but rather by player.
    // Should the inventory be bound to character or just player overall?
    public void OnEnable()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();
    }

    // When should the RuneSlots apply stats to the player.
    // When the slot is not empty.

    // How can I tell if the player has dropped an item into this slot?
    // Add an event listener to the inventory system



    // Need a listener in the inventory system for whenever items are picked up
    // When an item is picked up, check if it originiated from a rune slot?
    // If it did, we need to remove the effects of that rune as it is no longer embedded.
    // If inventory sourceSlot is a rune slot && sourceSlot had an item, remove rune effects.
    // Probably do not need a listener since the inventory system handles externally
    public void RemoveRuneEffects()
    {
        if (HasItem)
        {
            RuneData rune = item as RuneData; // Convert the base class item into rune data
            if (rune != null)
            {
                rune.RemoveStats(ref stats);  // Pass the player to this function
            }
        }
    }

    public void ApplyRuneEffects()
    {
        if (HasItem)    // only apply effects if a rune exists in this slot
        {
            Debug.Log("[Gear]: Applied rune effects");
            RuneData rune = item as RuneData; // Convert the base class item into rune data
            if (rune != null)
            {
                rune.ApplyStats(ref stats);  // Pass the player to this function
            }
        }
        else
        {
            Debug.Log("[Gear]: Did not have a rune to apply effects");
        }
    }
}
