using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HendokInteract : InteractableObject
{
    public AudioClip berserkerClip;
    public AudioClip hunterClip;
    public AudioClip elementalistClip;
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

        GoX_Class currClass = PlayerManager.Instance.currentClass;
        switch (currClass)
        {
            case GoX_Class.BERSERKER:
                interactSource.clip = berserkerClip;
                break;
            case GoX_Class.HUNTER:
                interactSource.clip = hunterClip;
                break;
            case GoX_Class.ELEMENTALIST:
                interactSource.clip = elementalistClip;
                break;
        }

        if (interactSource != null)
        {
            if (interactSource.clip != null)
            {
                interactSource.Play();
            }
        }

        // if (sounds != null)
        // {
        //     int randSound = Random.Range(0, sounds.sounds.Length);
        //     AudioClip clip = sounds.sounds[randSound];
        //     interactSource.clip = clip;
            
        // }
        //GameManager.Instance.PauseGame();
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
