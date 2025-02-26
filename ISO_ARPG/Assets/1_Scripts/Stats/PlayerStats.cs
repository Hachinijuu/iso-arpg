using System.Collections.Generic;
using UnityEngine;
public class PlayerStats : EntityStats
{
    // Player Stats has an active tracker of what the player currently has, health, mana, and their stats.
    #region VARIABLES
    [SerializeField] CharacterClass playerClass;    // the class the player has selected

    [SerializeField] IdentityAbility ID_Fusion;             // the selected fusion to add passively

    // Any additional abilities the player can get through gameplay
    public List<PassiveAbility> passives;
    public List<Stat> statList;

    // Instead of casting abilities through the playerClass, load the abilities into this class

    //public List<Ability> Abilities { get { return abilities; } }
    public List<Ability> Abilities;

    //private List<Ability> abilities;
    public IdentityAbility Identity { get { return identity; } }
    IdentityAbility identity;


    #region Public Accessors
    public CharacterClass Class { get { return playerClass; } }
    public Ability Fusion { get { return ID_Fusion; } }

    // Tracked Stats
    public TrackedStat Mana { get { return mana; } }
    public TrackedStat ID_Bar { get { return idBar; } }

    // Main Stats
    public MainStat STR { get { return strength; } }
    public MainStat DEX { get { return dexterity; } }
    public MainStat INT { get { return intelligence; } }

    // Gameplay Stats
    public Stat IDGain { get { return idGain; } }
    public Stat Range { get { return attackRange; } }
    public Stat Projectiles { get { return numProjectiles; } }
    public Stat Chains { get { return numChains; } }

    // Utility
    public SubStat MoveSpeed { get { return moveSpeed; } }

    // Offensive
    public SubStat Damage { get { return damage; } }
    public SubStat AttackSpeed { get { return attackSpeed; } }
    public SubStat CritDamage { get { return critDamage; } }
    public SubStat CritChance { get { return critChance; } }
    #endregion

    #region Stats
    TrackedStat mana;
    TrackedStat idBar;

    MainStat strength;
    MainStat dexterity;
    MainStat intelligence;


    // Gameplay
    Stat idGain;
    Stat attackRange;
    Stat numProjectiles;
    Stat numChains;

    // Utility
    SubStat moveSpeed;
    Stat rotationSpeed;

    // Offensive
    SubStat damage;
    SubStat attackSpeed;
    SubStat critDamage;
    SubStat critChance;
    #endregion
    #endregion

    #region UNITY FUNCTIONS
    public void Start()
    {
        //LoadDefaultClassStats();
        //LoadAbilities();
        //LoadStats();
    }
    #endregion
    #region INITALIZATION

    public void InitializePlayerStats()
    {
        // this will load stats externally keeping player progress, for now just LoadStats wrapper
        //LoadStats();
        statList = new List<Stat>();
        LoadDefaultClassStats();
        LoadAbilities();
        FillStatList();

        health.Changed += context => { CheckDied(); };
    }

    public void OnDisable()
    {
        health.Changed -= context => { CheckDied(); };
    }

    public void FillStatList()
    {
        statList.Add(health);
        statList.Add(mana);
        statList.Add(idBar);
        statList.Add(strength);
        statList.Add(dexterity);
        statList.Add(intelligence);
        statList.Add(attackRange);
        statList.Add(numProjectiles);
        statList.Add(numChains);
        statList.Add(moveSpeed);
        statList.Add(idGain);
        statList.Add(damage);
        statList.Add(attackSpeed);
        statList.Add(critDamage);
        statList.Add(critChance);
        statList.Add(armour);
        statList.Add(dodge);
    }

    public void LoadStats()
    {
        //LoadDefaultMainStats();
        //LoadTrackedStats();
        //LoadSubstats();
    }

    public void LoadDefaultClassStats()
    {
        // Need to copy or create new values
        // Assigning = would just make the reference, and change the base

        // main tracking

        health = new TrackedStat(playerClass.Health);
        mana = new TrackedStat(playerClass.Mana);
        idBar = new TrackedStat(TrackedStatTypes.ID_BAR, playerClass.IdentityAbility.Cost, playerClass.IdentityAbility.Cost);

        // main stats
        strength = new MainStat(playerClass.Strength);
        dexterity = new MainStat(playerClass.Dexterity);
        intelligence = new MainStat(playerClass.Intelligence);

        // gameplay stats
        attackRange = new Stat(playerClass.Range);
        numProjectiles = new Stat(playerClass.Projectiles);
        numChains = new Stat(playerClass.Chains);

        // utility stats
        moveSpeed = new SubStat(playerClass.moveSpeed);
        idGain = new Stat(playerClass.ID_Gain);
        //rotationSpeed.Value += playerClass.rotationSpeed;

        // offensive stats
        damage = new SubStat(playerClass.Damage);
        // weapon damage

        // ability

        // damage 1
        // damage 2

        // ability calculating damage to deal
        // player's damage (base damage) + ability damage

        attackSpeed = new SubStat(playerClass.AttackSpeed);
        critDamage = new SubStat(playerClass.CritDamage);
        critChance = new SubStat(playerClass.CritChance);

        // defensive stats
        armour = new SubStat(playerClass.Armour);
        dodge = new SubStat(playerClass.Dodge);
    }

    public PlayerStats CopyFromStats(PlayerStats stats)
    {
        PlayerStats temp = new PlayerStats();
        temp = stats;
        return temp;
        //health = new TrackedStat(stats.health);
        //mana = new TrackedStat(stats.mana);
        //
        //strength = new MainStat(stats.strength);
        //dexterity = new MainStat(stats.dexterity);
        //intelligence = new MainStat(stats.intelligence);
        //
        //attackRange = new Stat(stats.attackRange);
        //numProjectiles = new Stat(stats.numProjectiles);
        //numChains = new Stat(stats.numChains);
        //
        //moveSpeed = new SubStat(stats.moveSpeed);
        //idGain = new Stat(stats.idGain);
        //
        //damage = new SubStat(stats.damage);
        //attackSpeed = new SubStat(stats.attackSpeed);
        //critDamage = new SubStat(stats.critDamage);
        //critChance = new SubStat(stats.critChance);
        //
        //armour = new SubStat(stats.armour);
        //dodge = new SubStat(stats.dodge);
    }

    public void LoadAbilities()
    {
        // Copying the list values into the playable
        foreach (Ability ab in playerClass.Abilities)
        {
            Abilities.Add(Instantiate(ab));
        }
        identity = Instantiate(playerClass.IdentityAbility);    // Create a 'new' identity based on what the class has for game use
    }

    //public void LoadDefaultMainStats()
    //{
    //    strength = 
    //    //strength = new MainStat(MainStatTypes.STRENGTH, playerClass.strength);
    //    //dexterity = new MainStat(MainStatTypes.DEXTERITY, playerClass.dexterity);
    //    //intelligence = new MainStat(MainStatTypes.INTELLIGENCE, playerClass.intelligence);
    //}
    //
    //public void LoadTrackedStats()
    //{
    //    //health = new TrackedStat(TrackedStatTypes.HEALTH, playerClass.health, playerClass.health);
    //    //mana = new TrackedStat(TrackedStatTypes.MANA, playerClass.mana, playerClass.mana);
    //    //idBar = new TrackedStat(TrackedStatTypes.ID_BAR, 0, playerClass.identityAbility.Cost);
    //}
    //
    //public void LoadSubstats()
    //{
    //    //moveSpeed = new SubStat(SubStatTypes.MOVE_SPEED, playerClass.moveSpeed);
    //}

    public void CheckDied()
    {
        if (health.Value <= 0)
        {
            GameManager.Instance.PlayerDied();  // Tell the game manager the player has died, this will handle the bulk of it?
        }
    }
    public void Respawn()
    {
        health.Value = health.MaxValue;
        mana.Value = mana.MaxValue;
        idBar.Value = 0;
    }
    #endregion

    #region SET

    public void SetFusion(IdentityAbility fusion)
    {
        // Instead of setting it like this, make a LOCAL copy so that it is persistent like the rest of the abilities
         
        ID_Fusion = Instantiate(fusion);    // Create a 'new' identity based on what the class has for game use
        //ID_Fusion.asFusion = true;
        //ID_Fusion.UseAbility(gameObject);
    }
    #endregion
}
