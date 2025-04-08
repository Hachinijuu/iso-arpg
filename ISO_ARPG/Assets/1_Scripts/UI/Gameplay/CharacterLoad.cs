using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoad : MonoBehaviour
{
    public void SetGhostCharacter()
    { 
        
    }

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

    public void SlotClicked()
    {
        //SaveSystem.Instance.LoadProfile();
        // Load the profile of the save slot
    }

    public void PopulateSlots()
    {
        // Create the prefab in the thing for each of the slots

        // Foreach save in the SaveSystem, build a slot and put it in the list
    }

    public void Start()
    {
        PopulateSlots();
    }
}
