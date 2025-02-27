using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    // Steps for character selection
    // No character exists in persistent scene
    // Menu Start -> Load to Hub.
    
    // Hub Load, This script activates, sees that no characterCLass is selected (GameManager)
    // Opens UI for character select, selected character will have camera show them, and player controls are enabled.

    // This is debug character select for temp purposes
    [SerializeField] GameObject panel;

    [SerializeField] Button berserker;
    [SerializeField] Button hunter;
    [SerializeField] Button elementalist;
    public void ClassClicked(GoX_Class clickedClass)
    {
        PlayerManager.Instance.ActivatePlayer(clickedClass);

        ShowCharacterSelection(false);
    }

    public void BerserkerClicked()
    {
        ClassClicked(GoX_Class.BERSERKER);
    }
    public void HunterClicked()
    {
        ClassClicked(GoX_Class.HUNTER);
    }

    public void ElementalistClicked()
    {
        ClassClicked(GoX_Class.ELEMENTALIST);
    }
    public void ShowCharacterSelection(bool value)
    {
        // When showing the character selection - disable all characters
        panel.SetActive(value);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Allow this popup if debug mode is enabled, otherwise don't handle this shortcut
            if (GameManager.Instance.currGameState == GameManager.GameState.PLAYING)
            {
                if (!panel.activeInHierarchy)
                {
                    //GameManager.Instance.PauseGame();
                    ShowCharacterSelection(true);
                }
                else
                {
                    ShowCharacterSelection(false);
                    //GameManager.Instance.UnpauseGame();
                }
            }
        }
    }
}
