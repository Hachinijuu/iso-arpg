using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    // Steps for character selection
    // No character exists in persistent scene
    // Menu Start -> Load to Hub.
    
    // Hub Load, This script activates, sees that no characterCLass is selected (GameManager)
    // Opens UI for character select, selected character will have camera show them, and player controls are enabled.

    // This is debug character select for temp purposes
    [SerializeField] GameObject panel;
    [SerializeField] PlayerController berserker;
    [SerializeField] PlayerController hunter;
    [SerializeField] PlayerController elementalist;

    public void BerserkerButton()
    {
        berserker.gameObject.SetActive(true);
        hunter.gameObject.SetActive(false);
        elementalist.gameObject.SetActive(false);
        GameManager.Instance.GetPlayerReferences();
    }
    public void HunterButton()
    {
        berserker.gameObject.SetActive(false);
        hunter.gameObject.SetActive(true);
        elementalist.gameObject.SetActive(false);
        GameManager.Instance.GetPlayerReferences();
    }
    public void ElementalistButton()
    {
        berserker.gameObject.SetActive(false);
        hunter.gameObject.SetActive(false);
        elementalist.gameObject.SetActive(true);
        GameManager.Instance.GetPlayerReferences();
    }
    // public void SwitchTo(CharacterClass _class)
    // {
    //     if (_class == berserker.Stats.Class)
    //     {

    //     }
    //     else if (_class == hunter.Stats.Class)
    //     {
    //         berserker.gameObject.SetActive(false);
    //         hunter.gameObject.SetActive(true);
    //         elementalist.gameObject.SetActive(false);
    //     }
    //     else if (_class == elementalist.Stats.Class)
    //     {
    //         berserker.gameObject.SetActive(false);
    //         hunter.gameObject.SetActive(false);
    //         elementalist.gameObject.SetActive(true);
    //     }
    // }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (GameManager.Instance.currGameState == GameManager.GameState.PLAYING)
            {
                if (!panel.activeInHierarchy)
                {
                    //GameManager.Instance.PauseGame();
                    panel.SetActive(true);
                }
                else
                {
                    panel.SetActive(false);
                    //GameManager.Instance.UnpauseGame();
                }
            }
        }
    }
}
