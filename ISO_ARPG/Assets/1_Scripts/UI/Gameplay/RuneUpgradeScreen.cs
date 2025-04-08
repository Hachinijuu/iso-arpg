using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuneUpgradeScreen : MonoBehaviour
{
    public ItemRarity rank; // this will be swapped via toggles
    [SerializeField] GameObject runeList;
    [SerializeField] public ItemSlot upgradeSlot;

    [SerializeField] int buildCost;
    [SerializeField] int baseCost;
    [SerializeField] float rarityCostModifier;

    [SerializeField] Button createButton;
    [SerializeField] Button upgradeButton;
    [SerializeField] Button destroyButton;

    [SerializeField] TMP_Text dustText;
    [SerializeField] TMP_Text createText;
    [SerializeField] TMP_Text upgradeText;
    [SerializeField] TMP_Text destroyText;

    public void Start()
    {
        Inventory.Instance.OnItemReleased += context => { UpdateGhostRune(); };
        Inventory.Instance.OnDustChanged += UpdateDustText;
        UpdateGhostRune();
    }
    public int CalculateCost()
    {
        return (int)(baseCost * (1 + (rarityCostModifier * (int)rank)));
    }

    public int CalculateCost(int nextRank)
    {
        return (int)(baseCost * (1 + (rarityCostModifier * nextRank)));
    }

    public void OnEnable()
    {
        UpdateGhostRune();
        UpdateDustText(Inventory.Instance.RuneDust);      
    }

    int upgradeCost;
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
            if (Inventory.Instance.RuneDust >= upgradeCost)
            {
                // If the rune exists, increase the rarity of the rune
                if (rune.rarity < ItemRarity.RELIC) // If its not already relic tier
                {
                    rune.rarity += 1;               // Increase the rarirty rank of the rune
                }

                Inventory.Instance.RuneDust -= upgradeCost;
                rune = RuneSystem.Instance.RollRune(rune, rune.rarity, true);
                upgradeSlot.SetItem(rune);
                UpdateGhostRune();
            }
            else
            {
                PublicEventManager.Instance.FireOnCannot();
            }
            
            // Otherwise, regardless of the rank increase, roll for the rune upgrade

            // Although this has a potential to roll LESS than what the rune already has...
            // Do we modify the range of this rune roll so that the upgraded rune is next tier + guaranteed better stats on the given rolls?
            //rune = RuneSystem.Instance.RollRune(rune, rune.rarity);
            // The rune in the slot should be upgraded
        }
        //upgradeCost = CalculateCost();
    }
    public void UpdateGhostRune()
    {
        if (upgradeSlot.HasItem)
        {
            ghostRune = upgradeSlot.item as RuneData;
            createButton.interactable = false;
            upgradeButton.interactable = true;
            destroyButton.interactable = true;
            upgradeCost = CalculateCost((int)(ghostRune.rarity + 1));
        }
        else
        {
            // If the user does not have enough dust to create the rune, then prevent the creation
            if (Inventory.Instance.RuneDust >= buildCost)
            {
                createButton.interactable = true;   // Can build, enough dust
            }
            else
            {
                createButton.interactable = false;  // Can't build, not enough dust
            }

            upgradeButton.interactable = false;
            destroyButton.interactable = false;
        }
        UpdateButtonText();
    }

    public void UpdateButtonText()
    {
        createText.text = "Create" + " (" + buildCost + ")";
        
        if (upgradeSlot.HasItem  && upgradeSlot.item is RuneData rs)
        {
            if (rs.rarity == ItemRarity.RELIC)
            {
                upgradeText.text = "Reroll" + " (" + upgradeCost + ")";
            }
            else
            {
                upgradeText.text = "Upgrade" + " (" + upgradeCost + ")";
            }
            destroyText.text = "Destroy" + " (" + rs.destroyAmount + ")";
        }
        else
        {
            upgradeText.text = "Upgrade";
            destroyText.text = "Destroy";
        }
    }
    public RuneData ghostRune;
    public void CreateClicked()
    {
        // Opens up a list of stats to select from
        // Stores the current stat on the button press

        // Show the rune rank indicator
        runeList.SetActive(true);
    }

    public void RuneClicked(RuneData data)
    {
        // Only allow a rune to be clicked if there is no rune occupying the slot already
        if (ghostRune != null && upgradeSlot.item != null ) { Debug.Log("[RuneUpgrades]: Slot is already occupied"); return; }
        RuneData createdRune = new RuneData();
        createdRune = RuneSystem.Instance.RollRune(data, ItemRarity.COMMON, false);  // Given the new rune instance

        ghostRune = createdRune;
        upgradeSlot.SetItem(ghostRune);

        // I create a rune
        // I place it in the ghost slot
        // THIS is the rune that is created, unless otherwise logic to prevent it
        // From here, this rune can be pulled out and used.
        // I can only create more runes from this menu, IF A RUNE DOES NOT EXIST IN THE SLOT

        // How to verify the rune creation, is there no confirmation button or such? Is there 2-step verification?
        runeList.SetActive(false);
    }
    
    public void SetRarity(ItemRarity rarity)
    {
        this.rank = rarity;
    }

    public void MainStatClicked(MainStatTypes type)
    {
        ghostRune = RuneSystem.Instance.CreateMainStatRune(rank, type);
    }

    public void SubStatClicked(SubStatTypes type)
    {
        ghostRune = RuneSystem.Instance.CreateSubStatRune(rank, type);
    }

    public void UpdateDustText(int value)
    {
        dustText.text = Inventory.Instance.RuneDust.ToString();
    }

    public void ConfirmClicked()
    {
        // Consume the cost of the rune creation, and add the item to their inventory
        int cost = CalculateCost();
        if (cost > Inventory.Instance.RuneDust) 
        { 
            if (PublicEventManager.Instance != null)
            {
                PublicEventManager.Instance.FireOnCannot();
            }
            return; 
        } 
        // If the player does not have enough dust, do nothing, disable button prematurely, play error(?)
        Inventory.Instance.AddItem(ghostRune);
        Inventory.Instance.RuneDust -= cost;
    }
    public void DismantleClicked()
    {        
        RuneData rune = upgradeSlot.item as RuneData;
        if (rune != null)
        {
            // Add dust to the player's inventory
            Inventory.Instance.RuneDust += rune.destroyAmount;
            Destroy(rune);
            upgradeSlot.SetItem(null);
            UpdateGhostRune();
        }
    }
}
