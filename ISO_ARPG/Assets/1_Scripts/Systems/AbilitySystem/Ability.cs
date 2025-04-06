using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Ability Event Arguments Class
// Allows additional arguments to be created as necessary
public class AbilityEventArgs
{
    public Ability Ability { get; private set; }
    public PlayerController Actor { get; private set; }
    public List<Hitbox> Hitboxes { get; set; }    // This is used for external listening
    public AbilityEventArgs()
    {
        Hitboxes = new List<Hitbox>();
        Ability = null;
        Actor = null;
    }
    public AbilityEventArgs(Ability ability)
    {
        Hitboxes = new List<Hitbox>();
        Ability = ability;
        Actor = null;
    }
    public AbilityEventArgs(Ability ability, PlayerController actor)
    {
        Hitboxes = new List<Hitbox>();
        Ability = ability;
        Actor = actor;
    }
}
public abstract class Ability : ScriptableObject
{
    #region VARIABLES
    #region Public Accessors
    public Sprite Icon { get { return abilityIcon; } }
    public string Name { get { return abilityName; } }
    public string Description { get { return abilityDescription; } }
    public bool UseCooldown { get { return useCooldown; } }
    public float Cooldown { get { return cooldown; } }
    public float CurrCooldown { get { return currCooldown; } set { currCooldown = value; FireCooldownChanged(currCooldown); } }
    public bool OnCooldown { get { return currCooldown > 0; } }
    public float Cost { get { return cost; } }
    public bool StopsMovement { get { return stopsMovement; } }
    public bool CheckRange { get { return checkRange; } }
    #endregion

    #region Descriptions
    [Header("Descriptors")]
    [SerializeField] protected Sprite abilityIcon = null;

    [SerializeField] protected string abilityName = "invalid";

    [SerializeField] protected string abilityDescription = "invalid";
    #endregion

    #region Timers 
    [Header("Timing")]
    [SerializeField] protected bool useCooldown = false;
    [SerializeField] protected float cooldown = -1f;        // the cooldown time before the skill can be used again
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField] protected float cost = -1f;            // the mana cost of the ability
    [SerializeField] protected bool stopsMovement = false;  // if the ability stops player movement
    //[SerializeField] protected bool channelAbility = false; // if the ability can be held (channeled)
    //[SerializeField] protected float interval = -1f;        // the interval of when to consume (channeled)

    [SerializeField] protected bool checkRange = false;

    [Header("Audio")]
    [SerializeField] protected AudioClip abilityActivated;

    // Internals
    protected float currCooldown;
    #endregion
    #endregion

    #region FUNCTIONALITY

    public virtual void InitAbility(AbilityEventArgs e)
    {
        // Initialize the ability here, this is where GetComponents from derived abilites should be setup (called on character load)
    }
    protected abstract void Fire(ref AbilityEventArgs e); // This function must be overriden by base classes, it should contain the actions / effects of the ability.

    // call this function in the input handler, if the user can use their ability it will be used, otherwise - output can't
    public virtual void UseAbility(AbilityEventArgs e)            // the actor is WHO used the ability
    {
        Fire(ref e);
        FireAbilityUsed(e);     // fire off the event with THIS ability, and the actor, allowing any listens to use this information
    }

    // This function does not need to be overriden, but if/when - it can be used for ability cleanup or decaying effects.
    public virtual void EndAbility(AbilityEventArgs e)
    { 
        FireAbilityEnded(e);
    }
    #endregion

    #region EVENT SETUP
    public delegate void AbilityUsed(AbilityEventArgs args);
    public event AbilityUsed onAbilityUsed;

    public delegate void AbilityEnded(AbilityEventArgs args);
    public event AbilityEnded onAbilityEnded;

    public delegate void CooldownChanged(float value);
    public event CooldownChanged onCooldownChanged;

    protected void FireAbilityUsed(AbilityEventArgs e) { if (onAbilityUsed != null) onAbilityUsed(e); }
    protected void FireAbilityEnded(AbilityEventArgs e) { if (onAbilityEnded != null) onAbilityEnded(e); }
    protected void FireCooldownChanged(float value) { if (onCooldownChanged != null) onCooldownChanged(value); }


    #endregion
}
