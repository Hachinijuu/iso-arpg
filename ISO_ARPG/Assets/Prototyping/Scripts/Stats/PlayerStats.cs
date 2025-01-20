using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerStats : EntityStats
{
    // Player Stats has an active tracker of what the player currently has, health, mana, and their stats.
    #region VARIABLES
    [SerializeField] CharacterClass playerClass;    // the class the player has selected

    [SerializeField] Ability ID_Fusion;             // the selected fusion to add passively

    // Any additional abilities the player can get through gameplay
    public List<PassiveAbility> passives;

    // Instead of casting abilities through the playerClass, load the abilities into this class

    //public List<Ability> Abilities { get { return abilities; } }
    public List<Ability> Abilities;
    
    //private List<Ability> abilities;
    public Ability Identity { get { return identity; } }
    Ability identity;
    
    
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
    public Stat Projectiles { get { return numProjectiles; } }
    public Stat Chains { get { return numChains; } }

    // Utility
    public SubStat MoveSpeed { get { return moveSpeed; } }

    // Offensive
    public SubStat Damage { get { return damage; } }
    public SubStat AttackSpeed { get { return attackSpeed; } }
    public SubStat CritDamage { get { return critDamage; } }
    public SubStat CritChance { get { return critChance; } }

    // Defensive
    public SubStat Armour {  get { return armour; } }
    public SubStat Dodge { get { return dodge; } }  


    #endregion

    #region Stats
    TrackedStat mana;
    TrackedStat idBar;

    MainStat strength;
    MainStat dexterity;
    MainStat intelligence;


    // Gameplay
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

    // Defensive
    SubStat armour;
    SubStat dodge;
    #endregion

    public void Start()
    {
        //LoadDefaultClassStats();
        //LoadAbilities();
        //LoadStats();
    }

    public void InitializePlayerStats()
    {
        // this will load stats externally keeping player progress, for now just LoadStats wrapper
        //LoadStats();
        LoadDefaultClassStats();
        LoadAbilities();
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
        idBar = new TrackedStat(TrackedStatTypes.ID_BAR, 0, playerClass.IdentityAbility.Cost);

        // main stats
        strength = new MainStat(playerClass.Strength);
        dexterity = new MainStat(playerClass.Dexterity);
        intelligence = new MainStat(playerClass.Intelligence);

        // gameplay stats
        numProjectiles = new Stat(playerClass.Projectiles);
        numChains = new Stat(playerClass.Chains);

        // utility stats
        moveSpeed = new SubStat(playerClass.moveSpeed);
        //rotationSpeed.Value += playerClass.rotationSpeed;

        // offensive stats
        damage = new SubStat(playerClass.Damage);
        attackSpeed = new SubStat(playerClass.AttackSpeed);
        critDamage = new SubStat(playerClass.CritDamage);
        critChance = new SubStat(playerClass.CritChance);

        // defensive stats
        armour = new SubStat(playerClass.Armour);
        dodge = new SubStat(playerClass.Dodge);
        


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

    #endregion
}
