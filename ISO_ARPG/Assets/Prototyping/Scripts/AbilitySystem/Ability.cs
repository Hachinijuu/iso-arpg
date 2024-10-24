using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Ability", menuName = "sykcorSystems/Abilities", order = 2)]
public class Ability : ScriptableObject
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

    #endregion

    #region Descriptions
    [Header("Descriptors")]
    [SerializeField]
    protected Image abilityIcon = null;

    [SerializeField]
    protected string abilityName = "invalid";

    [SerializeField]
    protected string abilityDescription = "invalid";
    #endregion

    #region Timers 
    [Header("Timing")]
    [SerializeField]                    // the cooldown time before the skill can be used again
    protected float cooldown = -1f;

    [SerializeField]                    // the active time window of the skills activity
    protected float activeTime = -1f;
    #endregion

    #region Settings
    [Header("Settings")]
    [SerializeField]
    protected float cost = -1f;         // the mana cost of the ability
    [SerializeField]
    protected bool stopsMovement = false;
    #endregion
    #endregion

    #region FUNCTIONALITY
    // call this function in the input handler, if the user can use their ability it will be used, otherwise - output can't
    public virtual void UseAbility(GameObject actor)        // the actor is WHO used the ability
    {
        AbilityAction(actor);           // tell the action who used the ability
        FireAbilityUsed(this, actor);   // fire off the event with THIS ability, and the actor, allowing any listens to use this information
    }
    protected virtual void AbilityAction(GameObject actor) // write the ability functionality here -- would be abstract but Scriptable Objects canot use abstract functions
    {
    }
    #endregion

    #region EVENT SETUP
    public delegate void AbilityUsed(Ability used, GameObject actor = null);
    public event AbilityUsed onAbilityUsed;

    void FireAbilityUsed(Ability used, GameObject actor) { if (onAbilityUsed != null) onAbilityUsed(used, actor); }

    #endregion
}
