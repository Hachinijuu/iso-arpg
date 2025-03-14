using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneUpgradeScreen : MonoBehaviour
{

    [SerializeField] RuneSlot upgradeSlot;
    public void UpgradeClicked()
    {
        // When the upgrade button is clicked
        // This button should only be enabled it the player has a rune in the rune slot
        // And if the player has enough dust to upgrade the rune

        // Therefore, add listeners to the inventory system which will determine whether the button can be active or not

        // This function can include additional checks, but disable from event listening should be sufficient

        RuneData rune = upgradeSlot.item as RuneData;   // Get the rune from the slot
        if (rune != null)
        {
            // If the rune exists, increase the rarity of the rune
            if (rune.rarity < ItemRarity.RELIC) // If its not already relic tier
            {
                rune.rarity += 1;               // Increase the rarirty rank of the rune
            }
            
            // Otherwise, regardless of the rank increase, roll for the rune upgrade

            // Although this has a potential to roll LESS than what the rune already has...
            // Do we modify the range of this rune roll so that the upgraded rune is next tier + guaranteed better stats on the given rolls?
            rune = RuneSystem.Instance.RollRune(rune, rune.rarity);
            // The rune in the slot should be upgraded
        }
    }

    public void DismantleClicked()
    {
        RuneData rune = upgradeSlot.item as RuneData;
        if (rune != null)
        {
            // Add dust to the player's inventory
            Inventory.Instance.RuneDust += rune.destroyAmount;
        }
    }
}
