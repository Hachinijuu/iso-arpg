using System.Collections;
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
    [Header("Movement Stats")]
    public float moveSpeed = -1f;
    public float rotationSpeed = -1f;

    [Header("Stats")]
    public float health = -1f;
    public float mana = -1f;
    public float strength = -1f;
    public float dexterity = -1f;
    public float intelligence = -1f;
    #endregion

    #region Abilities
    [Header("Abilities")]
    public List<Ability> abilities = new List<Ability>();
    public Ability identityAbility = null;
    public float identityCost;                  // what needs to be built up in order to use the identity skill
    #endregion
    #endregion
}
