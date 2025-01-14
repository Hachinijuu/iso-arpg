using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Class", menuName = "sykcorSystems/CharacterClass", order = 1)]
public class CharacterClass : ScriptableObject
{
    // this character class scriptable object will serve as a blueprint for following class assets to be created from.
    // any realtime gameplay information will be controlled in playerdata / player controller.
    // THIS IS WHAT THE BASE CLASS (DEFAULT) STATS WILL BE

    // variables
    #region VARIABLES
    #region Class Description
    [Header("Descriptions")]
    public string className;

    public string classDescription;

    public Image classIcon;
    #endregion
    // maximum values
    //#region Constants
    //public const float MAX_SPEED = 100;
    //public const float MAX_ROTSPEED = 100;
    //
    //public const float MAX_HEALTH = 1000000;
    //public const float MAX_MANA = 1000000;
    //public const float MAX_STRENGTH = 1000000;
    //public const float MAX_AGILITY = 1000000;
    //public const float MAX_WISDOM = 1000000;
    //#endregion

    #region Stats

    [Header("Main Stats")]
    public TrackedStat Health;
    public TrackedStat Mana;
    [Range(0, StatLimits.STAT_MAX)] public float strength = -1f;
    [Range(0, StatLimits.STAT_MAX)] public float dexterity = -1f;
    [Range(0, StatLimits.STAT_MAX)] public float intelligence = -1f;

    [Header("Gameplay Stats")]

    [Header("Defensive Stats")]

    [Header("Offensive Stats")]

    [Header("Movement Stats")]
    [Range(0, StatLimits.MOVE_MAX)] public float moveSpeed = -1f;
    [Range(0, StatLimits.MOVE_MAX)] public float rotationSpeed = -1f;
    #endregion

    #region Abilities
    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();
    public Ability identityAbility = null;
    //public float identityCost;                  // what needs to be built up in order to use the identity skill
    #endregion
    #endregion
}
