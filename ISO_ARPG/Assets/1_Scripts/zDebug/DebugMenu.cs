using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    private static DebugMenu instance;
    public static DebugMenu Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DebugMenu>();
            if (!instance)
                Debug.LogWarning("[DebugMenu]: No Debug Menu found");
            return instance;
        }
    }

    [SerializeField] GameObject debugUI;    // this will be activated by a key press, it will contain the UI elements that the rest of the menu uses

    public bool debugOn;

    // When invincible, the player will not die even if their health reaches 0 or below - no death screen
    public bool Invincible { get { return invincible; } }
    bool invincible;

    // When take damage is set to false, player hurtbox will be disabled, preventing any damage from being taken whatsoever
    public bool TakeDamage { get { return takeDamage; } }
    bool takeDamage = true;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            if (debugUI == null)
                return;
            if (!debugUI.activeInHierarchy)
                debugUI.SetActive(true);
            else
                debugUI.SetActive(false);
        }
    }
}
