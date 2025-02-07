using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionSelection : MonoBehaviour
{
    public bool shouldConfirm;
    public GameObject selectPopup;
    private Ability fusion;
    public void SelectionClicked(Ability selectedFusion)
    {
        fusion = selectedFusion;
        if (selectPopup != null && shouldConfirm)
        {
            ShowPopup(true);
        }
        else
        {
            ConfirmSelection();
        }
        // When the selection is clicked, slot it into the player and prompt menu
        // Once popup has been confirmed, load into the level

        // Alternatively, no confirm popup and transition straight into level
    }

    private void ShowPopup(bool show)
    {
        selectPopup.SetActive(show);
    }
    public void ConfirmSelection()
    {
        // Load it to the player
        GameManager.Instance.Player.Stats.SetFusion(fusion);
        // Load the level

    }
}
