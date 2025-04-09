using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FusionSelection : MonoBehaviour
{
    public bool shouldConfirm;
    public GameObject selectPopup;
    private IdentityAbility fusion;
    public GameObject Rage;
    public GameObject Swiftness;
    public GameObject Gift;

    public void OnEnable()
    {
        CheckForClass();
        TutorialManager.Instance.BeginIdentityTutorial();
    }

    public void CheckForClass()
    {
        //Disable same class buttons
        switch(PlayerManager.Instance.currentClass)
        {
            case GoX_Class.BERSERKER:
                Rage.SetActive(false);
            break;
            case GoX_Class.HUNTER:
                Swiftness.SetActive(false);
            break;
            case GoX_Class.ELEMENTALIST:
                Gift.SetActive(false);
            break;
        }
    }
    public void SelectionClicked(IdentityAbility selectedFusion)
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
        //GameManager.Instance.Player.Stats.Fusion.UseAbility(GameManager.Instance.Player.gameObject); // this is pending
        // Load the level
        GameManager.Instance.LoadLevelByID(GameManager.eLevel.LEVEL_2);
    }
}
