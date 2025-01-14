using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    // Player Stats has an active tracker of what the player currently has, health, mana, and their stats.
    #region VARIABLES
    [SerializeField] CharacterClass playerClass;    // the class the player has selected

    [SerializeField] Ability ID_Fusion;             // the selected fusion to add passively

    #region Public Accessors
    public CharacterClass Class { get { return playerClass; } }
    public Ability Fusion { get { return ID_Fusion; } }
    public TrackedStat Health { get { return health; } }
    public TrackedStat Mana { get { return mana; } }
    public TrackedStat ID_Bar { get { return idBar; } }

    public MainStat STR { get { return strength; } }
    public MainStat DEX { get { return dexterity; } }
    public MainStat INT { get { return intelligence; } }

    public SubStat MoveSpeed { get { return moveSpeed; } }

    #endregion

    #region Stats
    TrackedStat health;
    TrackedStat mana;
    TrackedStat idBar;

    MainStat strength;
    MainStat dexterity;
    MainStat intelligence;


    // Gameplay
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
        LoadDefaultClassStats();
        //LoadStats();
    }

    public void InitializePlayerStats()
    {
        // this will load stats externally keeping player progress, for now just LoadStats wrapper
        //LoadStats();
    }

    public void LoadStats()
    {
        //LoadDefaultMainStats();
        //LoadTrackedStats();
        //LoadSubstats();
    }

    public void LoadDefaultClassStats()
    {
        // main tracking
        health = playerClass.Health;
        mana = playerClass.Mana;
        idBar = new TrackedStat(TrackedStatTypes.ID_BAR, 0, playerClass.identityAbility.Cost);

        // main stats
        strength = playerClass.Strength;
        dexterity = playerClass.Dexterity;
        intelligence = playerClass.Intelligence;

        // gameplay stats
        numProjectiles = playerClass.Projectiles;
        numChains = playerClass.Chains;

        // utility stats
        moveSpeed = playerClass.moveSpeed;
        rotationSpeed.Value = playerClass.rotationSpeed;

        // offensive stats
        damage = playerClass.Damage;
        attackSpeed = playerClass.AttackSpeed;
        critDamage = playerClass.CritDamage;
        critChance = playerClass.CritChance;

        armour = playerClass.Armour;
        dodge = playerClass.Dodge;
        
        // defensive stats


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
