using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    [Header("Options")]
    // move these options to game manager
    public bool useMana;
    public bool useCooldowns;

    #region VARIABLES
    PlayerStats stats;
    PlayerInput input;
    PlayerMovement movement;

    // public variables
    // accessors
    public bool isAttacking { get { return attacking; } }
    
    // private variables
    bool attacking = false;

    Dictionary<Ability, bool> canUseAbility = new Dictionary<Ability, bool>();
    #endregion

    // input handling
    #region INITALIZATION
    private void Awake()
    {
        // getting references to necessary components on this object
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();

        MapPlayerActions();
        InitalizePlayer();
    }
    void MapPlayerActions()
    {
        input.actions["Ab1"].performed += context =>
        {
            UseAbility(0);
        };
        input.actions["Ab2"].performed += context =>
        {
            UseAbility(1);
        };
        input.actions["IDab"].performed += context =>
        {
            UseIdentityAbility();
        };
    }

    void UnmapPlayerActions()
    {
        input.actions["Ab1"].performed -= context => { UseAbility(0); };
        input.actions["Ab2"].performed -= context => { UseAbility(1); };
        input.actions["IDab"].performed -= context => { UseIdentityAbility(); };
    }

    public void InitalizePlayer()           // this can be used for reinit, so be cautious of what to place here.
    {
        if (stats != null)
            stats.InitializePlayerStats();
        else
            Debug.LogError("Player has no stats to read from!");

        attacking = false;

        if (stats.Class.abilities.Count > 0)
            InitAbilities();
        else
            Debug.LogWarning("No abilities to read from");
    }

    public void InitAbilities()
    {
        foreach (Ability ab in stats.Class.abilities)                   // adding all of the abilities to the dictionary
        {
            canUseAbility.Add(ab, true);
        }
        canUseAbility.Add(stats.Class.identityAbility, true);           // adding the identity ability to the dictionary
    }
    #endregion

    private void Update()
    {
        
    }

    #region PLAYER ACTIONS
    // Handle targetting


    #region Ability System
    void UseAbility(int index)
    {
        if (stats != null)
        {
            if (stats.Class.abilities != null && stats.Class.abilities.Count > 0)   // if the class has abilities
            {
                if (index < stats.Class.abilities.Count)                            // if the requested ability is within the list of abilities
                {
                    Ability ability = stats.Class.abilities[index];                 // get a reference to the ability

                    // different settings -- organized in each option for easier logic breakdown
                    if (useMana && useCooldowns)
                    {
                        if (canUseAbility[ability])                 // ability is not on cooldown, first step of usage
                        {
                            if ((stats.Mana.Value - ability.Cost) < 0)    // not enough mana to use the ability
                            {
                                Debug.Log("Not enough mana to use: " + ability.Name + ", it costs: " + ability.Cost);
                            }
                            else                                    // enough mana to use the ability
                            {
                                ability.UseAbility(gameObject);             // use the ability
                                Debug.Log("Used: " + ability.Name);
                                canUseAbility[ability] = false;             // flag it as used
                                stats.Mana.Value -= ability.Cost;   // consume mana
                                if (ability.StopsMovement)
                                {
                                    StartCoroutine(HandleStop(ability));
                                }
                                StartCoroutine(HandleCooldown(ability));    // start the timer
                            }

                        }
                        else
                            Debug.Log(ability.Name + " is on cooldown...");
                    }
                    else if (useMana)
                    {
                        if ((stats.Mana.Value - ability.Cost) < 0)    // not enough mana to use the ability
                        {
                            Debug.Log("Not enough mana to use: " + ability.Name + ", it costs: " + ability.Cost);
                        }
                        else                                    // enough mana to use the ability
                        {
                            ability.UseAbility(gameObject);             // use the ability
                            Debug.Log("Used: " + ability.Name);
                            stats.Mana.Value -= ability.Cost;   // consume mana
                            if (ability.StopsMovement)
                                StartCoroutine(HandleStop(ability));
                        }
                    }
                    else if (useCooldowns)
                    {
                        if (canUseAbility[ability])                     // ability is not on cooldown
                        {
                            ability.UseAbility(gameObject);             // use the ability
                            Debug.Log("Used: " + ability.Name);
                            if (ability.StopsMovement)
                                StartCoroutine(HandleStop(ability));
                            canUseAbility[ability] = false;             // set it as used (on cooldown)
                            StartCoroutine(HandleCooldown(ability));    // start the timer
                        }
                        else
                            Debug.Log(ability.Name + " is on cooldown...");
                    }
                    else // no setting specified
                    {
                        ability.UseAbility(gameObject); // use the ability without any penalty
                        Debug.Log("Used: " + ability.Name);
                        if (ability.StopsMovement)
                            StartCoroutine(HandleStop(ability));
                    }
                }
                else
                    Debug.LogWarning("Requested ability index: " + index + ", is out of range");
            }
            else
                Debug.LogWarning("No ability found at the given index: " + index);
        }
        else
            Debug.LogWarning("Player has no stats to read abilities from!");
    }

    void UseIdentityAbility()
    {
        if (stats != null)
        {
            if (stats.Class.identityAbility != null)
            {
                // check if the player has enough mana/gauge built up
                if (canUseAbility[stats.Class.identityAbility])    // if the ability can be used
                {
                    if (stats.ID_Bar.Value == stats.ID_Bar.MaxValue)                    // check if there is enough identity gauge built up to use the skill
                    {
                        stats.Class.identityAbility.UseAbility(gameObject);                 // use the ability
                        Debug.Log("Used: " + stats.Class.identityAbility.Name);
                        if (useCooldowns)
                        { 
                            canUseAbility[stats.Class.identityAbility] = false;                 // set flag to indicate it has been used
                            StartCoroutine(HandleCooldown(stats.Class.identityAbility));    // start the clock for the countdown
                        }
                    }
                    else
                        Debug.Log("Identity gauge has not been built up enough");
                }
            }
            else
                Debug.LogWarning("No identity ability exists");
        }
        else
            Debug.LogWarning("Player has no stats to read abilities from!");
    }
    IEnumerator HandleStop(Ability abilityUsed)
    {
        movement.HandleStops(true);
        yield return new WaitForSeconds(abilityUsed.ActiveTime);
        movement.HandleStops(false);
    }
    IEnumerator HandleCooldown(Ability abilityUsed)
    {
        yield return new WaitForSeconds(abilityUsed.Cooldown);  // wait for the cooldown time --> actual time track can be abstracted to the things that need to read it (icon countdown)
        yield return canUseAbility[abilityUsed] = true;         // allow usage again
    }

    // DEBUG FUNCTIONS
    public void ResetAbilityUsage() // will reset used flags for all abilities
    {
        foreach (Ability ab in stats.Class.abilities)
        {
            canUseAbility[ab] = true;
        }
        canUseAbility[stats.Class.identityAbility] = true;
    }

    #endregion
    #endregion
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}
