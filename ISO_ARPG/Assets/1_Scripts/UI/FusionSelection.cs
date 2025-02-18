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
        // Fire the fusion
        GameManager.Instance.Player.Stats.Fusion.UseAbility(GameManager.Instance.Player.gameObject); // this is pending
        // Load the level

    }
}
