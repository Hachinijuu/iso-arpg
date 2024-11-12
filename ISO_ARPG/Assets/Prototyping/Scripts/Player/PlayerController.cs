using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Options")]
    public bool useMana;
    public bool useCooldowns;

    #region VARIABLES
    [SerializeField]
    PlayerStats stats;

    // input 
    PlayerInput input;

    public bool debugLogs = false;

    bool attacking = false;
    bool canMove = true;

    [SerializeField]
    private bool moveHeld = false;

    private Vector3 moveTarget;

    Dictionary<Ability, bool> canUseAbility = new Dictionary<Ability, bool>();
    #endregion

    // input handling
    #region INITALIZATION
    private void Awake()
    {
        if (input == null)
            input = GetComponent<PlayerInput>();

        // input is handled here instead of reinit, because reinit can be used on class switching maybe?
        if (input != null)
            MapPlayerActions();
        else
            Debug.LogError("No player input to map actions to!");

        InitalizePlayer();
    }
    public void MapPlayerActions()
    {
        // map player controls to the actions
        input.actions["MoveConfirmed"].canceled += context =>  // right click
        {
            moveHeld = false;
            if (canMove)
                GetMoveTarget();
        };
        input.actions["MoveConfirmed"].started += context =>
        {
            moveHeld = true;
            if (canMove)
                GetMoveTarget();
        };
        input.actions["StopMove"].started += context =>
        {
            HandleMoveLock(true);
        };
        input.actions["StopMove"].canceled += context =>
        {
            HandleMoveLock(false);
        };
        input.actions["BasicAttack"].performed += context =>
        {
            Attack();
        };
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

    public void InitalizePlayer()           // this can be used for reinit, so be cautious of what to place here.
    {
        if (stats != null)
            stats.InitializePlayerStats();
        else
            Debug.LogError("Player has no stats to read from!");

        attacking = false;
        canMove = true;

        if (stats.Class.Abilities.Count > 0)
            InitAbilities();
        else
            Debug.LogWarning("No abilities to read from");
    }

    public void InitAbilities()
    {
        foreach (Ability ab in stats.Class.Abilities)                   // adding all of the abilities to the dictionary
        {
            canUseAbility.Add(ab, true);
        }
        canUseAbility.Add(stats.Class.IdentityAbility, true);           // adding the identity ability to the dictionary
    }
    #endregion
    private void Update()
    {
        if (moveHeld && canMove)
        {
            GetMoveTarget();
        }
        if (moveTarget != Vector3.zero) // handling movement
        {
            if (canMove)
            {
                UpdateRotation();
                UpdatePosition();
            }
        }
    }

    #region MOVEMENT
    private void UpdateRotation()
    {
        // get the direction
        Vector3 dir = moveTarget - transform.position;
        dir.Normalize();
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, stats.Speed * Time.deltaTime);
    }
    private void UpdatePosition()
    {
        moveTarget.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, stats.RotationSpeed * Time.deltaTime);

        // if the destination is reached.
        if (transform.position == moveTarget)
        {
            moveTarget = Vector3.zero;
        }
    }

    // call this whenever the mouse is clicked
    void GetMoveTarget()
    {
        RaycastHit hit;

        // for the raycast call, experiment with the layer masking to only interact with the physics on the relevant layer
        // need to check if mouse is on valid location (ground layer)
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            if (debugLogs)
                Debug.Log("Hit: " + LayerMask.LayerToName(hit.transform.gameObject.layer));
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                moveTarget = hit.point;
                // initialize a game object or particle using this move target to show the player where there click will move them (after clicked, little indicator to show)

                if (debugLogs)
                    Debug.Log("Set move point to: " + moveTarget);
            }
            else
            {
                if (debugLogs)
                    Debug.Log("Not on the ground...");
            }
        }
    }

    void HandleMoveLock(bool locked)
    {
        if (locked)
        {
            canMove = false;
            moveTarget = transform.position; // reset target to where player is standing;
        }
        else
            canMove = true;
    }
    #endregion
    #region PLAYER ACTIONS
    void Attack()
    {
        if (debugLogs)
            Debug.Log("Attacking!");
    }
    #region Ability System
    void UseAbility(int index)
    {
        if (stats != null)
        {
            if (stats.Class.Abilities != null && stats.Class.Abilities.Count > 0)   // if the class has abilities
            {
                if (index < stats.Class.Abilities.Count)                            // if the requested ability is within the list of abilities
                {
                    Ability ability = stats.Class.Abilities[index];                 // get a reference to the ability

                    // different settings -- organized in each option for easier logic breakdown
                    if (useMana && useCooldowns)
                    {
                        if (canUseAbility[ability])                 // ability is not on cooldown, first step of usage
                        {
                            if ((stats.Mana - ability.Cost) < 0)    // not enough mana to use the ability
                            {
                                Debug.Log("Not enough mana to use: " + ability.Name + ", it costs: " + ability.Cost);
                            }
                            else                                    // enough mana to use the ability
                            {
                                ability.UseAbility(gameObject);             // use the ability
                                Debug.Log("Used: " + ability.Name);
                                canUseAbility[ability] = false;             // flag it as used
                                stats.SetMana(stats.Mana - ability.Cost);   // consume mana
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
                        if ((stats.Mana - ability.Cost) < 0)    // not enough mana to use the ability
                        {
                            Debug.Log("Not enough mana to use: " + ability.Name + ", it costs: " + ability.Cost);
                        }
                        else                                    // enough mana to use the ability
                        {
                            ability.UseAbility(gameObject);             // use the ability
                            Debug.Log("Used: " + ability.Name);
                            stats.SetMana(stats.Mana - ability.Cost);   // consume mana
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
            if (stats.Class.IdentityAbility != null)
            {
                // check if the player has enough mana/gauge built up
                if (canUseAbility[stats.Class.IdentityAbility])    // if the ability can be used
                {
                    if (stats.IdentityValue == stats.Class.IdentityCost)                    // check if there is enough identity gauge built up to use the skill
                    {
                        stats.Class.IdentityAbility.UseAbility(gameObject);                 // use the ability
                        Debug.Log("Used: " + stats.Class.IdentityAbility.Name);
                        if (useCooldowns)
                        { 
                            canUseAbility[stats.Class.IdentityAbility] = false;                 // set flag to indicate it has been used
                            StartCoroutine(HandleCooldown(stats.Class.IdentityAbility));    // start the clock for the countdown
                        }
                    }
                    else
                        Debug.Log("Identity gauge has not been built up enough");
                }
                else
                    if (debugLogs)
                    Debug.Log("Cannot use: " + stats.Class.IdentityAbility + ", it is flagged as used: " + canUseAbility[stats.Class.IdentityAbility]);
            }
            else
                Debug.LogWarning("No identity ability exists");
        }
        else
            Debug.LogWarning("Player has no stats to read abilities from!");
    }
    IEnumerator HandleStop(Ability abilityUsed)
    {
        canMove = false;
        yield return new WaitForSeconds(abilityUsed.ActiveTime);
        yield return canMove = true;
    }
    IEnumerator HandleCooldown(Ability abilityUsed)
    {
        yield return new WaitForSeconds(abilityUsed.Cooldown);  // wait for the cooldown time --> actual time track can be abstracted to the things that need to read it (icon countdown)
        yield return canUseAbility[abilityUsed] = true;         // allow usage again
    }

    // DEBUG FUNCTIONS
    public void ResetAbilityUsage() // will reset used flags for all abilities
    {
        foreach (Ability ab in stats.Class.Abilities)
        {
            canUseAbility[ab] = true;
        }
        canUseAbility[stats.Class.IdentityAbility] = true;
    }

    #endregion
    #endregion
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}
