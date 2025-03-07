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
    public SubStat IDGain { get { return idGain; } }
    public Stat Range { get { return attackRange; } }
    public SubStat Projectiles { get { return numProjectiles; } }
    public SubStat Chains { get { return numChains; } }

    // Utility
    public SubStat MoveSpeed { get { return moveSpeed; } }

    // Offensive
    //public SubStat Damage { get { return damage; } }
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
    SubStat idGain;
    Stat attackRange;
    SubStat numProjectiles;
    SubStat numChains;

    // Utility
    SubStat moveSpeed;

    // Offensive
    //SubStat damage;
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
        //LoadDefaultClassStats();
        LoadAbilities();
        FillStatList();

        //health.Changed += context => { CheckDied(context); };
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

    public override void LoadData(EntityData toLoad)
    {
        LoadDefaultClassStats();
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
        numProjectiles = new SubStat(playerClass.Projectiles);
        numChains = new SubStat(playerClass.Chains);

        // utility stats
        moveSpeed = new SubStat(playerClass.moveSpeed);
        idGain = new SubStat(playerClass.ID_Gain);
        //rotationSpeed.Value += playerClass.rotationSpeed;

        // offensive stats
        damage = new SubStat(playerClass.Damage);
        // weapon damage

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
    public override void CheckDied(float value)
    {
        if (DebugMenu.Instance)
        {
            if (DebugMenu.Instance.Invincible)  // If invincibility is on, then don't check for death
                return;
        }

        base.CheckDied(value);
    }
    public void Respawn()
    {
        health.Value = health.MaxValue;
        mana.Value = mana.MaxValue;
        idBar.Value = 0;
        Debug.Log("[PlayerStats]: Respawned, values set to default");
    }
    #endregion

    #region SET

    public void SetFusion(IdentityAbility fusion)
    {
        // Instead of setting it like this, make a LOCAL copy so that it is persistent like the rest of the abilities
        Debug.Log("[PlayerStats]: " + fusion.Name + " assigned to player, apply effects");
        ID_Fusion = Instantiate(fusion);    // Create a 'new' identity based on what the class has for game use
        ID_Fusion.asFusion = true;
        ID_Fusion.InitAbility(ID_Fusion, gameObject);
        ID_Fusion.UseAbility(gameObject);
        foreach (Ability ab in Abilities)
        {
            ab.InitAbility(ab, gameObject);
        }
        Debug.Log("[PlayerStats]: Finished setting up " + fusion.Name);
        //ID_Fusion.asFusion = true;
        //ID_Fusion.UseAbility(gameObject);
    }
    #endregion
}
