using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HendokInteract : InteractableObject
{
    protected override void InteractAction()
    {
        // When this is clicked, show the screen
        if (GameplayUIController.Instance.statsScreen.activeInHierarchy)
        {
            Inventory.Instance.ShowCharacterScreen(false);
        }
        if (!GameplayUIController.Instance.inventoryScreen.activeInHierarchy)
        {
            Inventory.Instance.ShowInventory(true);
        }
        GameplayUIController.Instance.ShowSmith();
        GameManager.Instance.PauseGame();
        // If the player moves too far, shut off the screen

        // When shutting the screen off, cleanup
    }

    // Collider space
    public void OnTriggerExit(Collider other)
    {
        GameplayUIController.Instance.HideSmith();
        Debug.Log("Exited Trigger");
    }
}
