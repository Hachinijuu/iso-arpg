using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] PlayerController player;

    [SerializeField] Slider healthSlider;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Slider manaSlider;
    [SerializeField] TMP_Text manaText;

    [SerializeField] UIAbility ab1;
    [SerializeField] UIAbility ab2;
    [SerializeField] UIAbility idAbility;

    private PlayerStats playerStats;    // Get a reference to the player's stat system

    // Get references to the relevant stats
    TrackedStat health;
    TrackedStat mana;
    #endregion
    #region UNITY FUNCTIONS
    // private void Start()
    // {
    //     PlayerManager.Instance.onPlayerChanged += context => { SetPlayer(context.player); };
    // }

    private void OnEnable() 
    {
        InitHud();    
    }
    void OnDisable()
    {
        RemoveEventListeners();
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
        RemoveEventListeners();
        InitHud();
    }
    public void InitHud()
    {
        if (player != PlayerManager.Instance.currentPlayer)
            player = PlayerManager.Instance.currentPlayer;
        //if (player == null)
        //{
        //    // If the player controller is not assigned in editor, find the component through the player
        //    //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //}

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

        if (healthText != null)
            UpdateHealthText(health.Value);

        if (manaText != null)
            UpdateManaText(mana.Value);

        LoadAbilities();
        AddEventListeners();
    }
    #endregion
    #region INTIALIZATION
    public void LoadAbilities()
    {
        if (playerStats != null)
        {
            ab1.LoadFromAbility(playerStats.Abilities[0]);
            ab2.LoadFromAbility(playerStats.Abilities[1]);
            idAbility.LoadFromAbility(playerStats.Identity);
        }
    }

    // EVENT MAPPING
    private void AddEventListeners()
    {
        playerStats.Health.Changed += context => { UpdateHealthSlider(context); UpdateHealthText(context); };
        playerStats.Mana.Changed += context => { UpdateManaSlider(context); UpdateManaText(context); };
        AddCooldownListeners();
    }

    private void RemoveEventListeners()
    {
        playerStats.Health.Changed -= UpdateHealthSlider;
        playerStats.Mana.Changed -= UpdateManaSlider;
        RemoveCooldownListeners();
    }

    public void AddCooldownListeners()
    {
        playerStats.Abilities[0].onCooldownChanged += context =>
        { UpdateCooldownSlider(ab1, context); ShowCooldownText(ab1, playerStats.Abilities[0]); UpdateCooldownText(ab1, context); };
        playerStats.Abilities[1].onCooldownChanged += context =>
        { UpdateCooldownSlider(ab2, context); ShowCooldownText(ab2, playerStats.Abilities[1]); UpdateCooldownText(ab2, context); };
        playerStats.Identity.onCooldownChanged += context =>
        { UpdateCooldownSlider(idAbility, context); ShowCooldownText(idAbility, playerStats.Identity); UpdateCooldownText(idAbility, context); };
    }

    public void RemoveCooldownListeners()
    {
        playerStats.Abilities[0].onCooldownChanged -= context => { UpdateCooldownSlider(ab1, context); UpdateCooldownText(ab1, context); };
        playerStats.Abilities[1].onCooldownChanged -= context => { UpdateCooldownSlider(ab2, context); UpdateCooldownText(ab2, context); };
        playerStats.Identity.onCooldownChanged -= context => { UpdateCooldownSlider(idAbility, context); UpdateCooldownText(idAbility, context); };
    }
    #endregion
    #region FUNCTIONALITY

    #region Hud Updates
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

    public void UpdateHealthText(float value)
    {
        healthText.text = value.ToString() + " / " + health.MaxValue;
    }

    public void UpdateManaText(float value)
    {
        manaText.text = value.ToString() + " / " + mana.MaxValue;
    }

    public void ShowCooldownText(UIAbility ui, Ability ab)
    {
        ui.cooldown.gameObject.SetActive(ab.OnCooldown);
    }

    public void UpdateCooldownSlider(UIAbility ui, float value)
    {
        ui.cooldown.text = value.ToString("F1");
    }

    public void UpdateCooldownText(UIAbility ui, float value)
    {
        ui.cd_slider.value = value;
    }
    #endregion
    #endregion
}
