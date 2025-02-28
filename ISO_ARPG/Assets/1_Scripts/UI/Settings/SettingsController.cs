using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    [SerializeField] GameObject settingsScreen;



    // Get references to each toggle
    // When the toggles are pressed, enable the other groups

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
}
