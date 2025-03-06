using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] GameObject settingsScreen;

    [SerializeField] Toggle gameToggle;
    [SerializeField] Toggle videoToggle;
    [SerializeField] Toggle audioToggle;
    [SerializeField] GameObject gameTab;
    [SerializeField] GameObject videoTab;
    [SerializeField] GameObject audioTab;



    // Get references to each toggle
    // When the toggles are pressed, enable the other groups

    // Each sub-tab will have it's own controller

    // Update is called once per frame
    void Update()
    {
        // Temp input detection, for debug purposes mostly
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!settingsScreen.activeInHierarchy)
            {
                settingsScreen.SetActive(true);
            }
            else
            {
                settingsScreen.SetActive(false);
            }
        }
    }

    public void Awake()
    {
        if (gameTab.activeInHierarchy)
        {
           gameToggle.isOn = true;
           TabClicked();
        }
    }

    // public void ToggleInteract(bool value)
    // {
    //     if (value)
    //     {

    //     }
    // }

    public void TabClicked()
    {
        if (gameToggle.isOn)
        {
            gameTab.SetActive(true);
            gameToggle.interactable = false;
        }
        else
        {
            gameTab.SetActive(false);
            gameToggle.interactable = true;
        }
        if (videoToggle.isOn)
        {
            videoTab.SetActive(true);
            videoToggle.interactable = false;
        }
        else
        {
            videoTab.SetActive(false);
            videoToggle.interactable = true;
        }
        if (audioToggle.isOn)
        {
            audioTab.SetActive(true);
            audioToggle.interactable = false;
        }
        else
        {
            audioTab.SetActive(false);
            audioToggle.interactable = true;
        }
    }
}
