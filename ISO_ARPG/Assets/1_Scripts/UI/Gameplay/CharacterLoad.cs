using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoad : MonoBehaviour
{
    public void CreateClicked()
    {
        GameManager.Instance.LoadNewCharacter();
    }
    
    public void PlayClicked()
    {
        // only if a player exists
        GameManager.Instance.LoadHub();
    }

    public void SettingsClicked()
    {
        GameplayUIController.Instance.settings.ShowSettingsScreen();
    }

    public void QuitClicked()
    {
        // Prompt an are you sure you want to leave
        GameManager.Instance.QuitGame();
    }
}
