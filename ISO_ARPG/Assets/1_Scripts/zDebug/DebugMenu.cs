using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    private void Awake()
    {
        invinciblityToggle.isOn = invincible;
        takeDamage = true;
        damageToggle.isOn = takeDamage;
    }

    [SerializeField] GameObject debugUI;    // this will be activated by a key press, it will contain the UI elements that the rest of the menu uses
    [SerializeField] Toggle invinciblityToggle;
    [SerializeField] Toggle damageToggle;
    [SerializeField] TMP_Dropdown levelDropdown;

    public bool debugOn;

    // When invincible, the player will not die even if their health reaches 0 or below - no death screen
    public bool Invincible { get { return invincible; } }
    bool invincible;

    // When take damage is set to false, player hurtbox will be disabled, preventing any damage from being taken whatsoever
    public bool TakeDamage { get { return takeDamage; } }
    bool takeDamage = true;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (debugUI == null)
                return;
            if (!debugUI.activeInHierarchy)
                debugUI.SetActive(true);
            else
                debugUI.SetActive(false);
        }
    }
    public void SetInvincible(bool value)
    {
        invincible = value;
        Debug.Log("[Debug]: Invinciblity: " + invincible);
    }

    public void SetDamageTake(bool value)
    {
        takeDamage = value;
        Debug.Log("[Debug]: Take Damage: " + takeDamage);
        HandleHurtboxes();
    }

    public void OnEnable()
    {
        if (levelDropdown != null)
            levelDropdown.value = (int)GameManager.Instance.level;
    }

    public void LevelHandler(Int32 value)
    {
        GameManager.Instance.level = (GameManager.eLevel)value;
        GameManager.Instance.LevelLoad();
        levelDropdown.value = (int)GameManager.Instance.level;
    }

    [SerializeField] Hurtbox[] playerHurtbox;

    public void HandleHurtboxes()
    {
        if (playerHurtbox != null && playerHurtbox.Length> 0)
        {
            foreach (Hurtbox hb in playerHurtbox)
            {
                if (hb.gameObject.transform.parent.gameObject.activeInHierarchy)
                {
                    hb.gameObject.SetActive(takeDamage);
                }
                // If you want to take damage, enable the hurtbox, otherwise do not
            }
        }
    }
}
