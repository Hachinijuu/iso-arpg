using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UIAbility
{
    public Image abImage;
    public TMP_Text name;
    public TMP_Text description;
    public TMP_Text cost;
    public TMP_Text cooldown;
    public void LoadFromAbility(Ability toLoad)
    {
        Debug.Log("[HUD]: Loading " + toLoad.name);
        abImage = toLoad.Icon;
        name.text = toLoad.Name;
        //description.text = toLoad.Description;
        //cost.text = toLoad.Cost;
        //cooldown.text = toLoad.Cooldown;
    }
}

public class HUDController : MonoBehaviour
{
    [SerializeField] PlayerController player;

    [SerializeField] Slider healthSlider;
    [SerializeField] Slider manaSlider;

    [SerializeField] UIAbility ab1;
    [SerializeField] UIAbility ab2;
    [SerializeField] UIAbility idAbility;

    private PlayerStats playerStats;    // Get a reference to the player's stat system

    // Get references to the relevant stats
    TrackedStat health;
    TrackedStat mana;

    private void Start()
    {
        if (player == null)
        { 
            // If the player controller is not assigned in editor, find the component through the player
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if (player != null)
        { 
            playerStats = player.Stats;
            health = playerStats.Health;
            mana = playerStats.Mana;
        }

        if (healthSlider != null)
            UpdateHealthSlider(health.Value);
        else    
            Debug.LogWarning("[HUD]: Health slider missing reference");

        if (manaSlider != null)
            UpdateManaSlider(mana.Value);
        else
            Debug.LogWarning("[HUD]: Health slider missing reference");

        AddEventListeners();
        LoadAbilities();
    }

    // EVENT FUNCTIONALITY
    private void AddEventListeners()
    {
        playerStats.Health.Changed += UpdateHealthSlider;
        playerStats.Mana.Changed += UpdateManaSlider;
    }

    private void RemoveEventListeners()
    { 
        playerStats.Health.Changed -= UpdateHealthSlider;
        playerStats.Mana.Changed -= UpdateManaSlider;
    }


    // UI FUNCTIONS
    public void UpdateHealthSlider(float value)
    {
        // With these changes, it is possible to interpolate from old value to new values (show the change gradually rather than snap)

        healthSlider.maxValue = health.MaxValue;
        healthSlider.value = value;
    }

    public void UpdateManaSlider(float value)
    { 
        manaSlider.maxValue = mana.MaxValue;
        manaSlider.value = value;
    }

    public void LoadAbilities()
    {
        if (playerStats != null)
        {
            ab1.LoadFromAbility(playerStats.Abilities[0]);
            ab2.LoadFromAbility(playerStats.Abilities[1]);
            idAbility.LoadFromAbility(playerStats.Identity);
        }
    }
}
