using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HendokInteract : InteractableObject
{
    protected override void InteractAction()
    {
        // When this is clicked, show the screen
        GameplayUIController.Instance.ShowSmith();
        // If the player moves too far, shut off the screen

        // When shutting the screen off, cleanup
    }

    public void LeaveInteract()
    {   
        GameplayUIController.Instance.HideSmith();

        // If too far, hide the screen and clean it 
    }

    // Collider space
    public void OnTriggerExit(Collider other)
    {
        GameplayUIController.Instance.HideSmith();
        Debug.Log("Exited Trigger");
    }
}
