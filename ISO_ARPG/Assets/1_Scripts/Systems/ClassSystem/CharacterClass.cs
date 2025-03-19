using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "sykcorSystems/Entities/Class", order = 2)]
public class CharacterClass : EntityData
{
    // this character class scriptable object will serve as a blueprint for following class assets to be created from.
    // any realtime gameplay information will be controlled in playerdata / player controller.
    // THIS IS WHAT THE BASE CLASS (DEFAULT) STATS WILL BE

    // variables
    #region VARIABLES
    #region Stats

    // Base Class Contains
    // Health
    // Damage
    // Attack Speed
    // Armour
    // Dodge
    // MoveSpeed

    [Header("CLASS STATS")]
    [Header("Main Stats")]
    public TrackedStat Mana;
    public MainStat Strength;
    public MainStat Dexterity;
    public MainStat Intelligence;

    [Header("Gameplay Stats")]
    public Stat Range;
    [Range(0, StatLimits.MOVE_MAX)] public float rotationSpeed = -1f;

    [Header("Offensive Stats")]
    //public SubStat AttackSpeed;
    public SubStat PrimaryDamage;
    public SubStat SecondaryDamage;
    public SubStat CritDamage;
    public SubStat CritChance;
    public SubStat Projectiles;
    public SubStat Chains;
    [Header("Defensive Stats")]
    public SubStat TakeFromMana;
    public SubStat HealthRegen;

    [Header("Utility Stats")]
    public SubStat CooldownReduction;
    //public SubStat moveSpeed;
    public SubStat ID_Gain;
    public SubStat ManaRegen;
    public SubStat ManaOnHit;

    #endregion

    #region Abilities
    [Header("Abilities")]
    public List<Ability> Abilities = new List<Ability>();
    public IdentityAbility IdentityAbility = null;
    //public float identityCost;                  // what needs to be built up in order to use the identity skill

    public float BaseIDGain = 5.0f; // The base ID gain generated per kill given this class

    #endregion
    #endregion
}
