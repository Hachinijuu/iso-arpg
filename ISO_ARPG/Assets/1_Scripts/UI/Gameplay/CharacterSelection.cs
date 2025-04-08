using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BodySelection
{
    public GameObject mainBody;
    public GameObject altBody;
}

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

    public List<BodySelection> bodies;
    public AudioClip[] clips;
    public AudioSource source;

    public void Start()
    {
        source = GameManager.Instance.GlobalAudio;
        BerserkerClicked(); // Sets the default character
    }

    public void SetGhostCharacter()
    {
        int index = (int)currentClass - 1;
        Debug.Log(currentClass + " : " + index);
        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].mainBody.SetActive(false);
            bodies[i].altBody.SetActive(false);
        }
        //foreach (BodySelection body in bodies)
        //{
        //    body.mainBody.SetActive(false);
        //}

        if (bodySelection == 0)
        {
            bodies[index].mainBody.SetActive(true);
        }
        else
        {
            bodies[index].altBody.SetActive(true);
        }

        if (source != null)
        {
            source.clip = clips[index];
            source.Play();
        }
    }
    public void BerserkerClicked()
    {
        bodySelection = 0;
        currentClass = GoX_Class.BERSERKER;
        PlayerManager.Instance.SetPlayer(currentClass, (GoX_Body)bodySelection);
        // Change the UI to display berserker information
        if (!debugSelection)
        {
            SetGhostCharacter();
            characterInfo.LoadFromClass(PlayerManager.Instance.currentPlayer.Stats.Class);
        }
    }
    public void HunterClicked()
    {
        bodySelection = 0;
        currentClass = GoX_Class.HUNTER;
        PlayerManager.Instance.SetPlayer(currentClass, (GoX_Body)bodySelection);

        // Change the UI to display hunter information
        //ClassClicked(GoX_Class.HUNTER);
        if (!debugSelection)
        {
            SetGhostCharacter();
            characterInfo.LoadFromClass(PlayerManager.Instance.currentPlayer.Stats.Class);
        }
    }

    public int bodySelection = 0;

    public void ElementalistClicked()
    {
        bodySelection = 0;
        currentClass = GoX_Class.ELEMENTALIST;
        PlayerManager.Instance.SetPlayer(currentClass, (GoX_Body)bodySelection);

        // Change the UI to display elementalist information
        //ClassClicked(GoX_Class.ELEMENTALIST);
        if (!debugSelection)
        {
            SetGhostCharacter();
            characterInfo.LoadFromClass(PlayerManager.Instance.currentPlayer.Stats.Class);
        }
    }

    public void ConfirmClicked()
    {
        //PlayerManager.Instance.ActivatePlayer(currentClass, GoX_Body.ONE);    // This transitions into gameplay activation
        PlayerManager.Instance.ActivatePlayer();
        if (!debugSelection)
        {
            GameManager.Instance.LoadLevelByID(GameManager.eLevel.CUTSCENE);
        }
        //ShowCharacterSelection(false);
    }

    public void AltArrowClicked()
    {
        bodySelection++;
        if (bodySelection == 2)
        {
            bodySelection = 0;
        }
        SetGhostCharacter();
        PlayerManager.Instance.SetPlayer(currentClass, (GoX_Body)bodySelection);
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
            if (Input.GetKeyDown(KeyCode.F4))
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
