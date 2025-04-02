using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    // This object is shutoff, figure out what's shutting it off
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
        takeDamage = true;
        invinciblityToggle.isOn = invincible;
        damageToggle.isOn = takeDamage;
    }

    [SerializeField] GameObject debugUI;    // this will be activated by a key press, it will contain the UI elements that the rest of the menu uses
    [SerializeField] Toggle invinciblityToggle;
    [SerializeField] Toggle damageToggle;
    [SerializeField] TMP_Dropdown levelDropdown;

    [SerializeField] PotionData healthPotion;
    [SerializeField] PotionData manaPotion;

    public enum DebugMath { ADD, SUBTRACT, MUTIPLY, DIVIDE };
    public DebugMath resourceMath;

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

    public void FillIdentity()
    {
        PlayerStats stats = PlayerManager.Instance.currentPlayer.Stats;
        if (stats != null)
        {
            stats.ID_Bar.Value = stats.ID_Bar.MaxValue;
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

    public void AddRune()
    {
        if (RuneSystem.Instance != null)
        {
            RuneData rune = RuneSystem.Instance.CreateMainStatRune(ItemRarity.COMMON, MainStatTypes.NONE);
            // This creates a rune and adds it to the inventory
            // The problem is the created run is fake
            if (rune != null)
            {
                Inventory.Instance.AddItem(rune);
            }
        }
    }

    public void AddHealthPotion()
    {
        // Need a reference to health potions
        Inventory.Instance.AddPotion(healthPotion);
    }

    public void AddManaPotion()
    {
        Inventory.Instance.AddPotion(manaPotion);
    }


    public void ResourceHandler(Int32 value)
    {
        resourceMath = (DebugMath)value;
    }
    [SerializeField] TMP_Dropdown resourceMathDropdown;
    [SerializeField] TMP_InputField woodInput;
    [SerializeField] TMP_InputField dustInput;
    [SerializeField] TMP_InputField stoneInput;

    // [SerializeField] Button woodSubmit;
    // [SerializeField] Button dustSubmit;
    // [SerializeField] Button stoneSubmit;

    public void WoodSubmit()
    {
        switch(resourceMath)
        {
            case DebugMath.ADD:
                Inventory.Instance.WoodAmount += int.Parse(woodInput.text);
            break;
            case DebugMath.SUBTRACT:
                Inventory.Instance.WoodAmount -= int.Parse(woodInput.text);
            break;
            case DebugMath.MUTIPLY:
                Inventory.Instance.WoodAmount *= int.Parse(woodInput.text);
            break;
            case DebugMath.DIVIDE:
                Inventory.Instance.WoodAmount /= int.Parse(woodInput.text);
            break;
        }
    }

    public void DustSubmit()
    {
        switch(resourceMath)
        {
            case DebugMath.ADD:
                Inventory.Instance.RuneDust += int.Parse(dustInput.text);
            break;
            case DebugMath.SUBTRACT:
                Inventory.Instance.RuneDust -= int.Parse(dustInput.text);
            break;
            case DebugMath.MUTIPLY:
                Inventory.Instance.RuneDust *= int.Parse(dustInput.text);
            break;
            case DebugMath.DIVIDE:
                Inventory.Instance.RuneDust /= int.Parse(dustInput.text);
            break;
        }
    }

    public void StoneSubmit()
    {
        switch(resourceMath)
        {
            case DebugMath.ADD:
                Inventory.Instance.StoneAmount += int.Parse(stoneInput.text);
            break;
            case DebugMath.SUBTRACT:
                Inventory.Instance.StoneAmount -= int.Parse(stoneInput.text);
            break;
            case DebugMath.MUTIPLY:
                Inventory.Instance.StoneAmount *= int.Parse(stoneInput.text);
            break;
            case DebugMath.DIVIDE:
                Inventory.Instance.StoneAmount /= int.Parse(stoneInput.text);
            break;
        }
    }


    [SerializeField] List<Hurtbox> playerHurtbox;

    public void HandleHurtboxes()
    {
        //if (playerHurtbox == null || playerHurtbox.Count <= 0)
        //{
        //    // Look for the hurtboxes from the Player Manager
        //    if (PlayerManager.Instance != null)
        //    {
        //        foreach (CharacterPair pair in PlayerManager.Instance.characters)
        //        { 
        //            playerHurtbox.Add(pair.Character.GetComponent<Hurtbox>());
        //        }
        //    }
        //}

        if (playerHurtbox != null && playerHurtbox.Count > 0)
        {
            for (int i = 0; i < playerHurtbox.Count; i++)
            {
                Hurtbox hb = playerHurtbox[i];
                if (hb == null)
                {
                    hb = PlayerManager.Instance.characters[i].Character.GetComponent<Hurtbox>();
                }
                if (hb != null)
                {
                    hb.enabled = takeDamage;
                }
            }
        }
    }
}
