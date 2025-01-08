using UnityEngine;
using UnityEngine.UI;

public class AbilityEventArgs
{
    public Ability Ability { get; private set; }
    //public AbilityAction Action { get; private set; }
    public GameObject Actor { get; private set; }

    public AbilityEventArgs()
    {
        Ability = null;
        //Action = null;
        Actor = null;
    }
    public AbilityEventArgs(Ability ability, /*AbilityAction action, */GameObject actor)
    {
        Ability = ability;
        //Action = action;
        Actor = actor;
    }
}

//[CreateAssetMenu(fileName = "Ability", menuName = "sykcorSystems/Abilities", order = 2)]
public abstract class Ability : ScriptableObject
{
    #region VARIABLES
    #region Public Accessors
    public Image Icon { get { return abilityIcon; } }
    public string Name { get { return abilityName; } }
    public string Description { get { return abilityDescription; } }
    public float Cooldown { get { return cooldown; } }
    public float ActiveTime { get { return activeTime; } }
    public float Cost { get { return cost; } }
    public bool StopsMovement { get { return stopsMovement; } }
    public bool Channelable { get { return channelAbility; } }
    public float Interval { get { return interval; } }

    #endregion

    #region Descriptions
    [Header("Descriptors")]
    [SerializeField] protected Image abilityIcon = null;

    [SerializeField] protected string abilityName = "invalid";

    [SerializeField] protected string abilityDescription = "invalid";
    #endregion

    #region Timers 
    [Header("Timing")]
    [SerializeField] protected float cooldown = -1f;        // the cooldown time before the skill can be used again

    [SerializeField] protected float activeTime = -1f;      // the active time window of the skills activity
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField] protected float cost = -1f;            // the mana cost of the ability
    [SerializeField] protected bool stopsMovement = false;  // if the ability stops player movement
    [SerializeField] protected bool channelAbility = false; // if the ability can be held (channeled)
    [SerializeField] protected float interval = -1f;        // the interval of when to consume (channeled)

    //[Header("Action")]
    //[SerializeField] protected AbilityAction action;
    #endregion
    #endregion

    #region FUNCTIONALITY
    // call this function in the input handler, if the user can use their ability it will be used, otherwise - output can't

    public abstract void Fire(Ability ab, GameObject actor);
    public virtual void UseAbility(GameObject actor)        // the actor is WHO used the ability
    {
        Fire(this, actor);        // Use the ability assigned to this info holder.
        FireAbilityUsed(new AbilityEventArgs(this,/* action, */actor));     // fire off the event with THIS ability, and the actor, allowing any listens to use this information
        //if (action != null)
        //{ 
        //}
    }
    #endregion

    #region EVENT SETUP
    public delegate void AbilityUsed(AbilityEventArgs args);
    public event AbilityUsed onAbilityUsed;

    void FireAbilityUsed(AbilityEventArgs e) { if (onAbilityUsed != null) onAbilityUsed(e); }

    #endregion
}
