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
    [SerializeField] Slider idSlider;

    [SerializeField] UIAbility ab1;
    [SerializeField] UIAbility ab2;
    [SerializeField] UIAbility idAbility;

    private PlayerStats playerStats;    // Get a reference to the player's stat system

    [SerializeField] TMP_Text healthPotionText;
    [SerializeField] TMP_Text manaPotionText;
    [SerializeField] TMP_Text objectiveText;

    // Get references to the relevant stats
    TrackedStat health;
    TrackedStat mana;
    TrackedStat idBar;
    #endregion
    #region UNITY FUNCTIONS
    // private void Start()
    // {
    //     PlayerManager.Instance.onPlayerChanged += context => { SetPlayer(context.player); };
    // }

    public TMP_Text difficultyText;
    public void UpdateDifficultyDisplay()
    {
        if (difficultyText != null) difficultyText.text = GameManager.Instance.currDifficulty.difficulty.ToString();
    }
    private void OnEnable()
    {
        InitHud();
        RemoveLevelObjective();
    }
    void OnDisable()
    {
        RemoveEventListeners();
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
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
            idBar = playerStats.ID_Bar;
        }
        UpdateHudValues();

        LoadAbilities();
        AddEventListeners();
    }

    public void UpdateHudValues()
    {
        UpdateDifficultyDisplay();

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

        if (healthPotionText != null && manaPotionText != null)
        {
            UpdatePotions();
        }
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

    public void OpenCharacter()
    {
        //GameplayUIController.Instance.statsScreen.SetActive(true);
        // if (GameplayUIController.Instance.statsScreen.activeInHierarchy)
        // { 
        //     GameplayUIController.Instance.statsScreen.SetActive(false);
        // }
        // else
        // {
        //     GameplayUIController.Instance.statsScreen.SetActive(true);
        // }
        GameplayUIController.Instance.ShowCharacterScreen();
    }

    public void OpenInventory()
    { 
        //GameplayUIController.Instance.inventoryScreen.SetActive(true);
        // if (GameplayUIController.Instance.inventoryScreen.activeInHierarchy)
        // {
        //     GameplayUIController.Instance.inventoryScreen.SetActive(false);
        // }
        // else
        // {
        //     GameplayUIController.Instance.inventoryScreen.SetActive(true);
        // }
        GameplayUIController.Instance.ShowInventory();
    }

    public void OpenSettings()
    { 
        GameplayUIController.Instance.ShowSettings();
        //GameplayUIController.Instance.settings.settingsScreen.SetActive(true);
        //GameplayUIController.Instance.settings.SetActive(true);
        // if (GameplayUIController.Instance.settings.settingsScreen.activeInHierarchy)
        // {
        //     GameplayUIController.Instance.settings.settingsScreen.SetActive(false);
        //     GameManager.Instance.UnpauseGame();
        // }
        // else
        // {
        //     GameplayUIController.Instance.settings.settingsScreen.SetActive(true);
        //     GameManager.Instance.PauseGame();
        // }
    }

    // EVENT MAPPING
    private void AddEventListeners()
    {
        if (healthSlider) playerStats.Health.Changed += context => { UpdateHealthSlider(context); UpdateHealthText(context); };
        if (manaSlider) playerStats.Mana.Changed += context => { UpdateManaSlider(context); UpdateManaText(context); };
        if (idSlider) playerStats.ID_Bar.Changed += context => { UpdateIdSlider(context); };
        if (healthPotionText && manaPotionText) AddPotionListeners();
        AddCooldownListeners();
    }

    private void RemoveEventListeners()
    {
        if (healthSlider) playerStats.Health.Changed -= UpdateHealthSlider;
        if (manaSlider) playerStats.Mana.Changed -= UpdateManaSlider;
        if (idSlider) playerStats.ID_Bar.Changed -= UpdateIdSlider;
        if (healthPotionText && manaPotionText) RemovePotionListeners();
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

    public void AddPotionListeners()
    {
        Inventory.Instance.onPotionChanged += UpdatePotions;
    }

    public void RemovePotionListeners()
    {
        Inventory.Instance.onPotionChanged -= UpdatePotions;
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
        healthText.text = value.ToString("F1") + " / " + health.MaxValue;
    }

    public void UpdateManaText(float value)
    {
        manaText.text = value.ToString("F1") + " / " + mana.MaxValue;
    }

    public void UpdateIdSlider(float value)
    {
        idSlider.maxValue = idBar.MaxValue;
        idSlider.value = value;
    }

    public void ShowCooldownText(UIAbility ui, Ability ab)
    {
        if (ab.UseCooldown)
        {
            ui.cooldown.gameObject.SetActive(ab.OnCooldown);
        }
    }

    public void UpdateCooldownSlider(UIAbility ui, float value)
    {
        ui.cooldown.text = value.ToString("F1");
    }

    public void UpdateCooldownText(UIAbility ui, float value)
    {
        ui.cd_slider.value = value;
    }

    public void UpdatePotions()
    {
        healthPotionText.text = Inventory.Instance.GetNumHealthPotions().ToString();
        manaPotionText.text = Inventory.Instance.GetNumManaPotions().ToString();
    }

    [SerializeField] GameObject objectivePanel;
    [SerializeField] TMP_Text objectiveCounter;

    public void Reload()
    {
        UpdateHudValues();
        UpdateAbilityDisplays(ab1, playerStats.Abilities[0]);
        UpdateAbilityDisplays(ab2, playerStats.Abilities[1]);
        UpdateAbilityDisplays(idAbility, playerStats.Identity);
    }

    public void UpdateAbilityDisplays(UIAbility uiAbility, Ability ab)
    {
        UpdateCooldownSlider(uiAbility, ab.CurrCooldown); 
        ShowCooldownText(uiAbility, ab); 
        UpdateCooldownText(uiAbility, ab.CurrCooldown);
    }

    public void ShowLevelObjectives()
    {
        objectivePanel.SetActive(true);
        ListenForDeaths();
        UpdateEnemyNumber();
    }
    public void RemoveLevelObjective()
    {
        objectivePanel.SetActive(false);
        UnlistenForDeaths();
    }

    public void ListenForDeaths()
    {
        AIManager.Instance.onEnemyDied += UpdateEnemyNumber;
    }

    public void UnlistenForDeaths()
    {
        AIManager.Instance.onEnemyDied -= UpdateEnemyNumber;
    }
    public void UpdateEnemyNumber()
    {
        if (LevelManager.Instance == null) { return; }

        if (LevelManager.Instance.type == LevelManager.LevelType.ELITE)
        {
            objectiveText.text = "Kill Elite Commander to Open Portal : ";
            objectiveCounter.text = LevelManager.Instance.numEliteKilled + " / " + LevelManager.Instance.Details.elitesToKill;
        }
        else
        {
            objectiveText.text = "Kill Enemies to Open Portal :";
            objectiveCounter.text =  LevelManager.Instance.numKilled + " / " + LevelManager.Instance.Details.enemiesToKill;
        }
        // Rework tracking to maintain enemy amount with remains logic
    }

    #endregion
    #endregion
}
