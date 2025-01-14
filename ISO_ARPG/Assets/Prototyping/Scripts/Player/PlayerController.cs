using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// Player Controller will assign all required components for ease of use.
// Add controller to prefab, get all necessary pieces.
[RequireComponent(typeof(PlayerStats))]             // Stats
[RequireComponent(typeof(PlayerInput))]             // Input
[RequireComponent(typeof(PlayerMovement))]          // Movement
[RequireComponent(typeof(PlayerAbilityHandler))]    // Abilities
[RequireComponent(typeof(MouseTarget))]             // Hover Targetting

[RequireComponent(typeof(AudioSource))]             // Audio
[RequireComponent(typeof(Animator))]                // Animations
public class PlayerController : MonoBehaviour
{
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

        //MapPlayerActions();
        InitalizePlayer();
    }

    public void InitalizePlayer()           // this can be used for reinit, so be cautious of what to place here.
    {
        if (stats != null)
            stats.InitializePlayerStats();
        else
            Debug.LogError("Player has no stats to read from!");
    }
    #endregion

    private void Update()
    {

    }

    #region PLAYER ACTIONS
    // Handle targetting

    public IEnumerator HandleStop(int duration)
    {
        movement.HandleStops(true);
        yield return new WaitForSeconds(duration);
        movement.HandleStops(false);
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
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.red);
    }
}
