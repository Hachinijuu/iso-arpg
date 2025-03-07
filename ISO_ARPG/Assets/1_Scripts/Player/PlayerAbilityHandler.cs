using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityHandler : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] bool showDebug = false;
    PlayerStats stats;
    PlayerInput input;
    MouseTarget mouseTarget;
    PlayerMovement movement;

    Dictionary<Ability, bool> canUseAbility = new Dictionary<Ability, bool>();
    bool held;
    float squareRange;
    #endregion
    #region ACTION MAPPING
    void MapPlayerActions()
    {
        //input.actions["Ab1"].started += context => { AbilityBegan(0);};
        //input.actions["Ab1"].canceled += context => { AbilityEnded(0);};
        //input.actions["Ab2"].started += context => { AbilityBegan(1);};
        //input.actions["Ab2"].canceled += context => { AbilityEnded(1);};
        //input.actions["IDab"].performed += context => { UseIdentityAbility(); };

        // Future: when mapping actions, account for which ability is in the respective HUD slot (NOT the class loading)
        // i.e HUD can load character information to display and record, input will be relative to the display for proper correspondence?

        // For now...

        // For single use abilities, can use .performed event
        // For channel abilities, need to listen to both .started and .canceled
        for (int i = 1; i <= stats.Abilities.Count; i++)
        {
            string keyName = "Ab" + i;
            Ability ab = stats.Abilities[i - 1];
            if (ab is ChannelAbility)
            {
                input.actions[keyName].started += context => { AbilityBegan(ab); };
                input.actions[keyName].canceled += context => { AbilityEnded(ab); };
            }
            else
                input.actions[keyName].performed += context => { AbilityBegan(ab); };
        }
        input.actions["IDab"].performed += context => { UseIdentityAbility(stats.Identity); };
    }

    void UnmapPlayerActions()
    {
        for (int i = 1; i <= stats.Abilities.Count; i++)
        {
            string keyName = "Ab" + i;
            Ability ab = stats.Abilities[i - 1];
            if (ab is ChannelAbility)
            {
                input.actions[keyName].started -= context => { AbilityBegan(ab); };
                input.actions[keyName].canceled -= context => { AbilityEnded(ab); };
            }
            else
                input.actions[keyName].performed -= context => { AbilityBegan(ab); };
        }
        input.actions["IDab"].performed -= context => { UseIdentityAbility(stats.Identity); };
    }
    #endregion
    #region UNITY FUNCTIONS
    private void Awake()
    {
        // ORDER MATTERS
        // Abilities must be initialized before mapping the inputs
        // This is because the functions that are mapped, respond to the types of abilities that are loaded.
        InitAbilityHandler();   // Get the references required
        // Actionable creation happens OnEnable, assuming player switches

        //MapPlayerActions();
    }

    public void InitAbilityHandler()
    {
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInput>();
        mouseTarget = GetComponent<MouseTarget>();
        movement = GetComponent<PlayerMovement>();

        // Listen for passive ability activation
        //if (stats != null)
        //{
        //    squareRange = stats.Range.Value * stats.Range.Value;
        //    if (stats.Abilities.Count > 0)
        //    {
        //        InitAbilities();
        //        //StartCoroutine(HandleFusion());
        //    }
        //    else
        //        Debug.LogWarning("[AbilityHandler]: No abilities to read from");
        //}
    }
    private void OnEnable() 
    {
        // Probably init abilities elsewhere (on start)
        // 
        StopAllCoroutines();        // Since whenever enabled, the player is reinitialized, stop all coroutines running on the thread
        InitAbilities();
        MapPlayerActions();    
    }

    private void OnDisable()
    {
        Debug.Log("Actions unmapped");
        UnmapPlayerActions();
        // Deactivate any existing abilities
        foreach (KeyValuePair<Ability, bool> abilityUsage in canUseAbility)
        {
            // If you can't use the ability because it has been activated
            if (abilityUsage.Value == false)
            {
                abilityUsage.Key.EndAbility(gameObject);  // Stop using the ability
            }
            // This will shrink the player if they have 
        }
    }
    #endregion

    public void AddPassiveListeners()
    {
        foreach (PassiveAbility pAb in stats.passives)
        {
            // Add listeners
            StartCoroutine(HandlePassive(pAb));
        }
    }
    public void RemovePassiveListeners()
    {

    }
    public void InitAbilities()
    {
        // Flush the list, this allows for class swap abilities to function
        canUseAbility.Clear();
        canUseAbility = new Dictionary<Ability, bool>();
        Debug.Log(canUseAbility.Count);
        if (canUseAbility.Count <= 0 || canUseAbility == null)
        {
            foreach (Ability ab in stats.Abilities)
            {
                canUseAbility.Add(ab, true);
                ab.InitAbility(ab, gameObject);
            }
            canUseAbility.Add(stats.Identity, true);
            stats.Identity.InitAbility(stats.Identity, gameObject);
        }
    }
    #region HELPER FUNCTIONS
    float GetSquareDistance(Vector3 start, Vector3 end)
    {
        return (start - end).sqrMagnitude;
    }
    #endregion
    #region FUNCTIONALITY - ABILITY HANDLING
    void AbilityBegan(Ability ab)
    {
        if (ab != null)
        {
            // Check if in range to use
            // Target distance
            // get the distance between the target and the player

            // Don't want the ability to be fired when I am not in range\

            // TEMPORARY SOLUTION
            if (ab.CheckRange)
            {
                if (movement.CanMove)
                {
                    Vector3 temp = transform.position;
                    if (mouseTarget.Target == null)
                    {
                        // If the target does not exist , send the range somewh
                        //temp = movement.MoveTarget;
                        return;
                    }
                    else
                        temp = mouseTarget.Target.transform.position;
                    float dist = GetSquareDistance(transform.position, temp);
                    //Debug.Log(dist + " from " + squareRange);
                    if (dist > squareRange && movement.CanMove)
                        return;
                }
            }

            // But if I am not in range and not moving, I want the ability to be fired

            // Check if the ability is not on cooldown
            if (canUseAbility[ab])
            {
                // Check if the player has enough mana to use the ability
                if ((stats.Mana.Value - ab.Cost) < 0) // If consumed mana results in less, not enough mana
                {
                    Debug.Log("[AbilityHandler]: Not enough mana to use " + ab.Name + " (" + stats.Mana.Value + " / " + ab.Cost + ")");
                    return; // Early return
                }
                // Player has enough mana to use the ability

                held = true;                    // Set value to keypressed (held)
                stats.Mana.Value -= ab.Cost;    // Consume the cost of mana
                ab.UseAbility(gameObject);      // Use the ability

                // Set the ability to used
                canUseAbility[ab] = false;

                // Check what type of ability it is (Channel vs Single)
                if (ab is ChannelAbility channelAb)   // If it is a Channel-able ability
                {
                    // Handle the consumption of the hold
                    IEnumerator held = HandleHeld(channelAb);   // Store a reference to the coroutine
                    StopCoroutine(held);                        // Stop it if it is running
                    StartCoroutine(held);                       // Start the held cycle
                }
                else if (ab is PassiveAbility passiveAb)
                {
                    StartCoroutine(HandlePassive(passiveAb));
                }
                else // Single press ability
                {
                    AbilityEnded(ab);   // End the ability early
                }
                if (showDebug) Debug.Log("[AbilityHandler] Used: " + ab.Name);
            }
            else
                Debug.Log("[AbilityHandler]: Cannot use: " + ab.Name);
        }
    }


    // ISSUE TO RESOLVE
    // While on cooldown, ability ended can be called again, causing the cooldown to restart.
    // Don't want to run cooldown unless ability was actually used.

    // Have it bound to performed actions -> can dictate if/when an actions is performed (complete)
    // For hold-bound events, the performed would be whenever the conditions is met...
    // But on release is not performed...

    void AbilityEnded(Ability ab)
    {
        // Cooldowns will be activated based on key release - DO NOT WANT THIS TO HAPPEN

        if (ab != null)
        {
            held = false;                       // Key is not held anymore
            ab.EndAbility(gameObject);          // Call the abilities' end
            StopCoroutine("HandleHeld");        // Stop the handle held coroutine
            // ONLY start the cooldown if the ability CAN BE USED
            Debug.Log("Ended");

            if (!ab.OnCooldown) // Only put it on cooldown if it's not already on cooldown
                StartCoroutine(HandleCooldown(ab)); // Start the cooldown for the ability
            if (showDebug) Debug.Log("[AbilityHandler]: " + ab.Name + " ended");    // Output ability ended
        }
    }

    void UseIdentityAbility(Ability ab)
    {
        if (ab != null)
        {
            if (canUseAbility[ab])
            {
                if (stats.ID_Bar.Value == stats.ID_Bar.MaxValue)                // Check if there is enough of the gauge built up
                {
                    ab.UseAbility(gameObject);          // Use the ability
                    canUseAbility[ab] = false;          // Flag usage
                    stats.ID_Bar.Value -= ab.Cost;      // Consume gauge
                    StartCoroutine(HandlePassive(ab as PassiveAbility));
                    StartCoroutine(HandleCooldown(ab));                 // Start handling the cooldown for this ability  
                    if (showDebug) Debug.Log("[AbilityHandler] Used: " + ab.Name);    // Output ability used
                }
            }
        }
    }

    // This function exists because value assignments ContextCallbacks is not very reliable (Only exists within the frame of the callback)
    // IEnumerator HandleFusion()
    // {
    //     do
    //     {
    //         yield return new WaitForSeconds(2);
    //     } while (stats.Fusion == null);
    //     if (stats.Fusion != null)
    //     {
    //         stats.Fusion.UseAbility(gameObject);
    //         Debug.Log("[PlayerAbilityHandler]: Fired: " + stats.Fusion.Name);
    //         foreach(Ability ab in stats.Abilities)
    //         {
    //             ab.InitAbility(ab, gameObject); // Reinit the ability (gain projectile sources if any)
    //         }
    //         Debug.Log("PlayerAbilityHandler]: Completed Initialization: " + stats.Fusion.Name);
    //     }
    // }

    IEnumerator HandleHeld(ChannelAbility used)
    {
        // Channeled abilities will consume cost every n seconds
        // This will be defined by the abilities interval
        do
        {
            yield return new WaitForSeconds(used.Interval);
            // Once the time has passed
            if (showDebug) Debug.Log("[AbilityHandler] Using " + used.Name);
            if ((stats.Mana.Value - used.Cost) < 0) // If the consumed mana would result in no remaining mana
            {
                if (showDebug) Debug.Log("[AbilityHandler]: No more mana to use: " + used.Name);
                AbilityEnded(used);                    // End the ability
            }
            else    // Remaining mana, consume it.
            {
                if (used.ManaPerTick)
                {
                    stats.Mana.Value -= used.Cost;          // Consume the cost of mana
                }
                used.OnTick();  // Call the tick action
            }
        } while (held);
        AbilityEnded(used);
    }

    IEnumerator HandlePassive(PassiveAbility used)
    {
        // when the selection happens, if the coroutine is NOT running, start the coroutine upon selecting
        if (used != null)
        {
            float intervalTime = 0;
            float activeTime = 0;
            do
            {
                yield return new WaitForEndOfFrame();
                intervalTime += Time.deltaTime;
                activeTime += Time.deltaTime;

                if (activeTime > used.ActiveTime)
                {
                    canUseAbility[used] = true; // Time has elapsed
                    used.EndAbility(gameObject);
                    if (showDebug) Debug.Log("[AbilityHandler]: " + used.Name + " ended");
                    break;                      // Exit the loop - coroutine finished
                }
                if (intervalTime > used.IntervalTime)
                {
                    used.OnTick();      // Call the tick action
                    if (showDebug) Debug.Log("[AbilityHandler]: " + used.Name + " tick");
                    intervalTime = 0;   // Reset the count
                }
            }
            while (!canUseAbility[used]); // While this ability cannot be used (because it has been used)
        }
    }

    IEnumerator HandleCooldown(Ability used)
    {
        // With this wrapper functionality, PlayerAbilityHandler do calculations here (if CDR) and have updated cooldown values.
        // If theres a reference to the HUD controller, then the HUD's cooldown can also be called here for 1:1 timer

        // FUNCTION REWRITTEN TO CONTAIN ACTIVE COOLDOWN INSTEAD OF WAIT FOR SECONDS
        // THIS ALLOWS FOR CENTRALIZED COOLDOWN NUMBERS, INSTEAD OF CAUSING EACH SCRIPT TO HANDLE THEIR OWN COOLDOWNS ON ABILITYENDED
        bool onCd = true;
        float currTime = used.Cooldown;
        do
        {
            currTime -= Time.deltaTime;
            used.CurrCooldown = currTime;
            if (currTime < 0)
                onCd = false;
            yield return new WaitForEndOfFrame();
        }
        while (onCd);

        //yield return new WaitForSeconds(used.Cooldown); // Wait for the cooldown time
        canUseAbility[used] = true;                     // Allow usage again (time has passed)
    }
    #endregion
}


// Ability System can be refactored to make use of bool events...
// IsPressed
// WasPressedThisFrame
// WasReleasedThisFrame

// BUT - since context callbacks are already setup, that is probably the way to go about it.
// Need to handle active event, NOT call CD reset if the cooldown is already ticking...