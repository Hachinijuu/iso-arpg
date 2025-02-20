using UnityEngine;
using UnityEngine.UI;

public class FusionSelection : MonoBehaviour
{
    public bool shouldConfirm;
    public GameObject selectPopup;
    private Ability fusion;
    public Button RageButton;
    public Button SwiftButton;
    public Button GiftButton;

    public void Start()
    {
        CheckForClass();
    }

    public void CheckForClass()
    {
        // Disable same class buttons
        switch(GameManager.Instance.playerClass)
        {
            case GameManager.eClass.BERSERKER:
                RageButton.interactable = false;
            break;
            case GameManager.eClass.HUNTER:
                SwiftButton.interactable = false;
            break;
            case GameManager.eClass.ELEMENTALIST:
                GiftButton.interactable = false;
            break;
        }
    }
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
        GameManager.Instance.LoadLevelByID(GameManager.eLevel.LEVEL_2);
    }
}
