using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterLoad : MonoBehaviour
{
    [SerializeField] GameObject playerSlotHolder;
    [SerializeField] GameObject playerSlotPrefab;

    public List<BodySelection> bodies;
    public void SetGhostCharacter(PlayerProfile profile)
    {
        int index = (int)profile.character.Guardian - 1;
        //Debug.Log(currentClass + " : " + index);
        for (int i = 0; i < bodies.Count; i++)
        {
            bodies[i].mainBody.SetActive(false);
            bodies[i].altBody.SetActive(false);
        }
        //foreach (BodySelection body in bodies)
        //{
        //    body.mainBody.SetActive(false);
        //}

        if (profile.character.Body == 0)
        {
            bodies[index].mainBody.SetActive(true);
        }
        else
        {
            bodies[index].altBody.SetActive(true);
        }
    }

    public void CreateClicked()
    {
        GameManager.Instance.LoadNewCharacter();
    }
    
    public void PlayClicked()
    {
        // only if a player exists
        if (PlayerManager.Instance.currentPlayer != null)
        {
            
            SetProfile();
            //PlayerManager.Instance.currentPlayer = SaveSystem.Instance.currentProfile.character.Character;
            PlayerManager.Instance.ActivatePlayer();
            GameManager.Instance.LoadHub();
        }
    }

    public void SetProfile()
    {
        GoX_Class loadedClass = SaveSystem.Instance.currentProfile.character.Guardian;
        GoX_Body loadedBody = SaveSystem.Instance.currentProfile.character.Body;
        PlayerManager.Instance.SetPlayer(loadedClass, loadedBody);
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

    public void SlotClicked(PlayerProfile profile)
    {
        //SaveSystem.Instance.LoadProfile();
        // Load the profile of the save slot
        SaveSystem.Instance.currentProfile = profile;
        SetProfile();
        SetGhostCharacter(profile);
    }
    public void PopulateSlots()
    {
        // Create the prefab in the thing for each of the slots

        // Foreach save in the SaveSystem, build a slot and put it in the list

        foreach (PlayerProfile profile in SaveSystem.Instance.playerSaves)
        {
            if (playerSlotPrefab != null)
            {
                GameObject slotObject = Instantiate(playerSlotPrefab, playerSlotHolder.transform);
                PlayerUISlot slot = slotObject.GetComponent<PlayerUISlot>();
                if (slot != null)
                {
                    slot.LoadPlayerSlot(profile);
                    slot.slotButton.onClick.AddListener(() => SlotClicked(profile));
                    //slot.slotButton.onClick.AddListener();
                }
            }
        }
    }

    public void OnDisable()
    {
        PlayerUISlot[] slots = playerSlotHolder.GetComponentsInChildren<PlayerUISlot>();
        if (slots != null && slots.Length > 0)
        {
            foreach (PlayerUISlot slot in slots)
            {
                slot.slotButton.onClick.RemoveAllListeners();
            }
        }
    }

    public void Start()
    {
        PopulateSlots();
        if (SaveSystem.Instance.playerSaves != null && SaveSystem.Instance.playerSaves.Count > 0)
        {
            SlotClicked(SaveSystem.Instance.playerSaves[0]);
        }
    }
}
