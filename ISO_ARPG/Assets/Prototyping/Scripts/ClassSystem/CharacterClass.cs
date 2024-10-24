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

    // public accessors
    #region PUBLIC ACCESSORS
    public string Name { get { return className; } }
    public string Description { get { return classDescription; } }
    #region Movement
    public float Speed { get { return moveSpeed; } }
    public float RotationSpeed { get { return rotationSpeed; } }
    #endregion
    #region Stats
    public float MaxHealth { get { return health; } }
    public float MaxMana { get { return mana; } }
    public float Strength { get { return strength; } }
    public float Agility { get { return agility; } }
    public float Wisdom { get { return wisdom; } }
    #endregion
    #region Abilities
    public List<Ability> Abilities { get { return abilities; } }
    public Ability IdentityAbility { get { return identityAbility; } }
    public float IdentityCost { get { return identityCost; } }
    #endregion
    #endregion

    // variables
    #region VARIABLES
    #region Class Description
    [Header("Descriptions")]
    [SerializeField]
    protected string className;

    [SerializeField]
    protected string classDescription;

    public Image classIcon;
    #endregion
    // maximum values
    #region Constants
    public const float MAX_SPEED = 100;
    public const float MAX_ROTSPEED = 100;

    public const float MAX_HEALTH = 1000000;
    public const float MAX_MANA = 1000000;
    public const float MAX_STRENGTH = 1000000;
    public const float MAX_AGILITY = 1000000;
    public const float MAX_WISDOM = 1000000;
    #endregion

    #region Stats
    [Header("Movement Stats")]
    [SerializeField] protected float moveSpeed = -1f;
    [SerializeField] protected float rotationSpeed = -1f;

    [Header("Stats")]
    [SerializeField] protected float health = -1f;
    [SerializeField] protected float mana = -1f;
    [SerializeField] protected float strength = -1f;
    [SerializeField] protected float agility = -1f;
    [SerializeField] protected float wisdom = -1f;
    #endregion

    #region Abilities
    [Header("Abilities")]
    [SerializeField] protected List<Ability> abilities = new List<Ability>();

    [SerializeField] protected Ability identityAbility = null;
    [SerializeField] protected float identityCost;                  // what needs to be built up in order to use the identity skill
    #endregion
    #endregion
}
