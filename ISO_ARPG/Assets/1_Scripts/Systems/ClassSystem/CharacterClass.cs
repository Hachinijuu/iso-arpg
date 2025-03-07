using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Class", menuName = "sykcorSystems/Entities/Class", order = 2)]
public class CharacterClass : EntityData
{
    // this character class scriptable object will serve as a blueprint for following class assets to be created from.
    // any realtime gameplay information will be controlled in playerdata / player controller.
    // THIS IS WHAT THE BASE CLASS (DEFAULT) STATS WILL BE

    // variables
    #region VARIABLES
    #region Stats

    [Header("Main Stats")]
    public TrackedStat Mana;
    public MainStat Strength;
    public MainStat Dexterity;
    public MainStat Intelligence;

    [Header("Gameplay Stats")]
    public Stat Range;
    public SubStat Projectiles;
    public SubStat Chains;

    [Header("Utility Stats")]
    public SubStat moveSpeed;
    public SubStat ID_Gain;
    [Range(0, StatLimits.MOVE_MAX)] public float rotationSpeed = -1f;

    [Header("Offensive Stats")]
    public SubStat AttackSpeed;
    public SubStat CritDamage;
    public SubStat CritChance;

    #endregion

    #region Abilities
    [Header("Abilities")]
    public List<Ability> Abilities = new List<Ability>();
    public IdentityAbility IdentityAbility = null;
    //public float identityCost;                  // what needs to be built up in order to use the identity skill
    #endregion
    #endregion
}
