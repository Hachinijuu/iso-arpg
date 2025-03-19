using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


// Player Controller will assign all required components for ease of use.
// Add controller to prefab, get all necessary pieces.
[RequireComponent(typeof(PlayerStats))]             // Stats
[RequireComponent(typeof(PlayerInput))]             // Input
[RequireComponent(typeof(PlayerMovement))]          // Movement
[RequireComponent(typeof(PlayerAbilityHandler))]    // Abilities
[RequireComponent(typeof(MouseTarget))]             // Hover Targetting
[RequireComponent(typeof(PlayerAudio))]             // Audio Handling
[RequireComponent(typeof(ProjectileSource))]        // Projectiles
[RequireComponent(typeof(FootStepHandler))]         // Footsteps
[RequireComponent(typeof(PlayerSlotSystem))]        // Slots
[RequireComponent(typeof(NavMeshAgent))]                // AGENT FOR COLLISION DETECTION -- 
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
    //public NavMeshAgent Agent { get { return agent; } }
    public PlayerAbilityHandler AbilityHandler { get { return handler; } }
    public MouseTarget PlayerTarget { get { return playerTarget; } }
    public PlayerAudio AudioController { get { return audioController; } }
    public AudioSource VoiceSource { get { return voiceSource; } }
    public AudioSource SFXSource { get { return sfxSource; } }
    public Animator Animator { get { return animator; } }
    public ProjectileSource ShootSource { get { return shootSource; } }
    public FootStepHandler FootSteps { get { return footSteps; } }
    public PlayerSlotSystem Slots { get { return slots; } }
    public GameObject Body { get { return body; } }

    public GameObject AltBody { get { return body; } }

    public NavMeshAgent Aget { get { return agent; } }
    PlayerStats stats;
    PlayerInput input;
    PlayerMovement movement;
    PlayerAbilityHandler handler;
    MouseTarget playerTarget;
    PlayerAudio audioController;
    [SerializeField] AudioSource voiceSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] Animator animator;
    ProjectileSource shootSource;
    FootStepHandler footSteps;
    PlayerSlotSystem slots;

    enum BodyNumber { MAIN, ALT }
    BodyNumber type;
    [SerializeField] GameObject body;
    [SerializeField] GameObject altBody;

    NavMeshAgent agent;

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
        if (handler == null)
            handler = GetComponent<PlayerAbilityHandler>();
        if (playerTarget == null)
            playerTarget = GetComponent<MouseTarget>();
        if (audioController == null)
            audioController = GetComponent<PlayerAudio>();

        if (shootSource == null)
            shootSource = GetComponent<ProjectileSource>();
        if (footSteps == null)
            footSteps = GetComponent<FootStepHandler>();
        if (slots == null)
            slots = GetComponent<PlayerSlotSystem>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();


        if (animator == null)
            Debug.LogWarning("[PlayerController]: Missing a reference to the ANIMATOR");

        if (body == null)
            Debug.LogWarning("[PlayerController]: Missing a reference to the PLAYER BODY");

        // NOTE: audio sources are now assigne manually, notify if there is no source found
        if (voiceSource == null)
            Debug.LogWarning("[PlayerController]: Missing a reference to the VOICE AUDIO SOURCE");
        if (sfxSource == null)
            Debug.LogWarning("[PlayerController]: Missing a reference to the EFFECTS AUDIO SOURCE");

        //MapPlayerActions();
        //InitalizePlayer();
    }

    public void EnablePlayer(bool shouldEnable)
    {
        body.SetActive(shouldEnable);
        input.enabled = shouldEnable;
        movement.enabled = shouldEnable;
        handler.enabled = shouldEnable;
        playerTarget.enabled = shouldEnable;
        audioController.enabled = shouldEnable;
        sfxSource.enabled = shouldEnable;
        voiceSource.enabled = shouldEnable;
        animator.enabled = shouldEnable;
        shootSource.enabled = shouldEnable;
        footSteps.enabled = shouldEnable;
        slots.enabled = shouldEnable;
        agent.enabled = shouldEnable;
    }


    private int respawnID = Animator.StringToHash("Respawn");
    public void Respawn()
    {
        if (transform.localScale != Vector3.one)
            transform.localScale = Vector3.one;
        EnablePlayer(true);
        //animator.SetTrigger(respawnID);
        stats.Respawn();

        // Reset the ability handler
        handler.Respawn();
        movement.Respawn();
    }

    public void Died()
    {
        EnablePlayer(false);
    }

    #endregion
    #region INITALIZATION
    public void InitializePlayer()           // this can be used for reinit, so be cautious of what to place here.
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
    //public void SetBody(BodyNumber body)
    //{
    //    switch (body)
    //    {
    //        case BodyNumber.ALT:
    //            if (body)
    //            altBody.SetActive(true); 
    //        break;
    //        case BodyNumber.MAIN:
    //        default:
    //            
    //        break;
    //    }
    //}
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
        input.actions["Potion1"].performed += context => { UsePotion(PotionTypes.HEALTH); };  // Map whichever potion
        input.actions["Potion2"].performed += context => { UsePotion(PotionTypes.MANA); };  // Map whichever potion
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
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.red);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AIManager.MELEE_RANGE);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AIManager.RANGED_RANGE);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AIManager.CHASE_RANGE);
    }
    #endregion
}
