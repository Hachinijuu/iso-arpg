using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterUIPanel
{
    public TMPro.TMP_Text characterName;
    public TMPro.TMP_Text description;
    public Image characterIcon;
    public Image ab1Icon;
    public Image ab2Icon;

    public void LoadFromClass(CharacterClass toLoad)
    {
        characterName.text = toLoad.entityName;
        description.text = toLoad.description;
        characterIcon.sprite = toLoad.icon;
        

        // How to match ability icons to the class
        // Hardcode it
        ab1Icon.sprite = toLoad.Abilities[0].Icon;
        ab2Icon.sprite = toLoad.Abilities[1].Icon;
    }
}
public class CharacterSelection : MonoBehaviour
{
    // Steps for character selection
    // No character exists in persistent scene
    // Menu Start -> Load to Hub.
    
    // Hub Load, This script activates, sees that no characterCLass is selected (GameManager)
    // Opens UI for character select, selected character will have camera show them, and player controls are enabled.

    // This is debug character select for temp purposes
    public bool debugSelection;
    [SerializeField] GameObject panel;
    [SerializeField] CharacterUIPanel characterInfo;

    // Character selection will work slightly different
    // On the initial button click, the displayed class information will change
    // And the activate player will be relative to the displayed class

    // Once the character has been confirmed
    // Then handle the gameplay transition

    public GoX_Class currentClass;

    // public void ClassClicked(GoX_Class clickedClass)
    // {
    //     PlayerManager.Instance.ActivatePlayer(clickedClass);

    //     ShowCharacterSelection(false);
    // }
    public void BerserkerClicked()
    {
        currentClass = GoX_Class.BERSERKER;
        PlayerManager.Instance.SetPlayer(currentClass);
        // Change the UI to display berserker information
        if (!debugSelection)
            characterInfo.LoadFromClass(PlayerManager.Instance.currentPlayer.Stats.Class);
    }
    public void HunterClicked()
    {
        currentClass = GoX_Class.HUNTER;
        PlayerManager.Instance.SetPlayer(currentClass);
        // Change the UI to display hunter information
        //ClassClicked(GoX_Class.HUNTER);
        if (!debugSelection)
            characterInfo.LoadFromClass(PlayerManager.Instance.currentPlayer.Stats.Class);
    }

    public void ElementalistClicked()
    {
        currentClass = GoX_Class.ELEMENTALIST;
        PlayerManager.Instance.SetPlayer(currentClass);
        // Change the UI to display elementalist information
        //ClassClicked(GoX_Class.ELEMENTALIST);
        if (!debugSelection)
            characterInfo.LoadFromClass(PlayerManager.Instance.currentPlayer.Stats.Class);
    }

    public void ConfirmClicked()
    {
        PlayerManager.Instance.ActivatePlayer(currentClass);    // This transitions into gameplay activation
        ShowCharacterSelection(false);
    }
    public void ShowCharacterSelection(bool value)
    {
        // When showing the character selection - disable all characters
        panel.SetActive(value);
    }
    public void Update()
    {
        if (debugSelection)
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
}
