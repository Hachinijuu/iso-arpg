using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


// Player Controller will assign all required components for ease of use.
// Add controller to prefab, get all necessary pieces.
[RequireComponent(typeof(PlayerStats))]             // Stats
[RequireComponent(typeof(PlayerInput))]             // Input
[RequireComponent(typeof(PlayerMovement))]          // Movement
[RequireComponent(typeof(PlayerAbilityHandler))]    // Abilities
[RequireComponent(typeof(MouseTarget))]             // Hover Targetting
[RequireComponent(typeof(PlayerAudio))]             // Audio Handling

[RequireComponent(typeof(AudioSource))]             // Voice Source
[RequireComponent(typeof(Animator))]                // Animations
[RequireComponent(typeof(ProjectileSource))]        // Projectiles
[RequireComponent(typeof(FootStepHandler))]

// music / ambience - own sources / persistent
// UI - persistent / general
// effects / voice

public class PlayerController : MonoBehaviour
{
    // Public References
    // Any classes that need to access player values can find through GameManager's static reference

    #region VARIABLES
    public PlayerStats Stats { get { return stats; } }
    public PlayerInput Input { get { return input; } }
    public PlayerMovement Movement { get { return movement; } }
    public NavMeshAgent Agent { get { return agent; } }
    public PlayerAbilityHandler AbilityHandler { get { return handler; } }
    public MouseTarget PlayerTarget { get { return playerTarget; } }
    public PlayerAudio AudioController { get { return audioController; } }
    public AudioSource AudioSource { get { return audioSource; } }
    public Animator Animator { get { return animator; } }
    public ProjectileSource ShootSource { get { return shootSource; } }
    public FootStepHandler FootSteps { get { return footSteps; } }
    PlayerStats stats;
    PlayerInput input;
    PlayerMovement movement;
    NavMeshAgent agent;
    PlayerAbilityHandler handler;
    MouseTarget playerTarget;
    PlayerAudio audioController;
    AudioSource audioSource;
    Animator animator;
    ProjectileSource shootSource;
    FootStepHandler footSteps;

    [SerializeField] GameObject auraSource;
    #endregion

    // input handling
    #region UNITY FUNCTIONS
    private void Awake()
    {
        // getting references to necessary components on this object
        if (stats == null)
            stats = GetComponent<PlayerStats>();
        if (input == null)
            input = GetComponent<PlayerInput>();
        if (movement == null)
            movement = GetComponent<PlayerMovement>();
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (handler == null)
            handler = GetComponent<PlayerAbilityHandler>();
        if (playerTarget == null)
            playerTarget = GetComponent<MouseTarget>();
        if (audioController == null)
            audioController = GetComponent<PlayerAudio>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (shootSource == null)
            shootSource = GetComponent<ProjectileSource>();
        if (footSteps == null)
            footSteps = GetComponent<FootStepHandler>();

        //MapPlayerActions();
        //InitalizePlayer();
    }

    //public void Start()
    //{
    //    InitalizePlayer();
    //}

    public void EnablePlayer(bool shouldEnable)
    {
        //stats.enabled = shouldEnable;
        input.enabled = shouldEnable;
        movement.enabled = shouldEnable;
        agent.enabled = shouldEnable;
        handler.enabled = shouldEnable;
        playerTarget.enabled = shouldEnable;
        audioController.enabled = shouldEnable;
        audioSource.enabled = shouldEnable;
        animator.enabled = shouldEnable;
        shootSource.enabled = shouldEnable;
        footSteps.enabled = shouldEnable;
    }


    private int respawnID = Animator.StringToHash("Respawn");
    public void Respawn()
    {
        if (transform.localScale != Vector3.one)
            transform.localScale = Vector3.one;
        EnablePlayer(true);
        //animator.SetTrigger(respawnID);
        stats.Respawn();
        movement.Respawn();
    }

    public void Died()
    {
        EnablePlayer(false);
    }

    #endregion
    #region INITALIZATION
    public void InitalizePlayer()           // this can be used for reinit, so be cautious of what to place here.
    {
        if (stats != null)
            stats.InitializePlayerStats();
        else
            Debug.LogError("Player has no stats to read from!");

        //EnablePlayer(false);
    }
    #endregion

    #region PLAYER ACTIONS
    // Handle targetting
    public IEnumerator HandleStop(int duration)
    {
        movement.HandleStops(true);
        yield return new WaitForSeconds(duration);
        movement.HandleStops(false);
    }

    public void SetAura(bool value)
    {
        if (auraSource)
        {
            auraSource.SetActive(value);
        }
    }


    private void MapPlayerActions()
    {
        input.actions["Potion1"].performed += context => { UsePotion(PotionTypes.HEALTH); };  // Map whichever potion
        input.actions["Potion2"].performed += context => { UsePotion(PotionTypes.MANA); };  // Map whichever potion
    }

    private void UnmapPlayerActions()
    { 
        
    }

    // Potion Usage
    public void UsePotion(PotionTypes potion)
    {
        switch (potion)
        { 
            case PotionTypes.HEALTH:
                // Get the health stat, increase the health stat
                break;
            case PotionTypes.MANA:
                // Get the mana stat, increase the mana stat
                break;
        }

        // Try using the potion
        // If the potion cannot be used, play cannot use sound

        // If the potion count & potion exists
        // Consume the potion and increase the respective stat

        // Otherwise
        // Do not consume (no potion to use)
        // and play the unable sound

        // Where are potions stored
    }

    #endregion
    #region DEBUG
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.red);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyController.MELEEATTACK_DIST);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, EnemyController.RANGEATTACK_DIST);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, EnemyController.CHASE_DIST);
    }
    #endregion
}
