using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityHandler : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] bool showDebug = false;
    PlayerController controller;
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


        // ALL ABILITIES HAVE HELD FUNCTIONALITY

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
        if (controller == null)
            controller = GetComponent<PlayerController>();
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
    // private void OnEnable() 
    // {
    //     // Probably init abilities elsewhere (on start)
    //     // 
    //     StopAllCoroutines();        // Since whenever enabled, the player is reinitialized, stop all coroutines running on the thread
    //     InitAbilities();
    //     MapPlayerActions();    
    // }

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
                abilityUsage.Key.EndAbility(new AbilityEventArgs(abilityUsage.Key, controller));  // Stop using the ability
            }
            // This will shrink the player if they have 
        }
    }

    // ON ENABLE ON DISABLE SELECTED WILL BREAK GAME LOGIC IF THE PLAYER RETURNS TO THE MENU
    // IN THE FACT THAT ABILITIES WILL BE REINIALIZED AND SET TO VALUES
    // BEFORE, LEVEL LOADS WOULD CAUSE DUPLICATES (i.e) load to levels, calling abilities multiple times even though one click (has copys)
    // Now, there are no copys or shouldn't be any copys -- Not this fix

    public void PlayerSelected()
    {
        StopAllCoroutines();        // Since whenever enabled, the player is reinitialized, stop all coroutines running on the thread
        InitAbilities();
        MapPlayerActions();    
    }

    public void PlayerDeselected()
    {
        Debug.Log("Actions unmapped");
        UnmapPlayerActions();
        // Deactivate any existing abilities
        foreach (KeyValuePair<Ability, bool> abilityUsage in canUseAbility)
        {
            // If you can't use the ability because it has been activated
            if (abilityUsage.Value == false)
            {
                abilityUsage.Key.EndAbility(new AbilityEventArgs(abilityUsage.Key, controller));  // Stop using the ability
            }
            // This will shrink the player if they have 
        }
    }
    #endregion

    public void Respawn()
    {
        ResetCooldowns();
        Debug.Log("[AbilityHandler]: Respawn, Cooldowns reset");
    }

    public void ResetCooldowns()
    {
        StopAllCoroutines();    // THIS MIGHT BREAK THE PASSIVE HANDLING, SIMPLY RESTART PASSIVES ON RESPAWN
        // foreach (KeyValuePair<Ability, bool> abilityUsage in canUseAbility)
        // {
        //     Debug.Log(canUseAbility[abilityUsage.Key]);
        //     canUseAbility[abilityUsage.Key] = true;     // Allow the ability to be used
        //     //abilityUsage.Key.CurrCooldown = 0.0f;       // Reset the clock
        // }
        foreach (Ability ab in stats.Abilities)
        {
            canUseAbility[ab] = true;
            ab.CurrCooldown = 0.0f;
        }
    }
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
        Debug.Log("[AbilityHandler] number of abilites :" + canUseAbility.Count);
        if (canUseAbility.Count <= 0 || canUseAbility == null)
        {
            foreach (Ability ab in stats.Abilities)
            {
                canUseAbility.Add(ab, true);
                ab.InitAbility(new AbilityEventArgs(ab, controller));
            }
            canUseAbility.Add(stats.Identity, true);
            stats.Identity.InitAbility(new AbilityEventArgs(stats.Identity, controller));
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
        // What does a basic input action need to account for
        // Ability begans need to know if the input is held

        // When I am holding this key, I am also moving.
        // If I happen to move over an enemy, do I begin shooting?

        // Sure.
        // For ranged attacks should I just stop movement?
        // No. - will right clicks work? Probably
        // If I am directional movement, then I want left click to work no matter what.
        
        // How do we take these things into consideration?

        

        if (ab != null)
        {
            // Check if in range to use
            // Target distance
            // get the distance between the target and the player

            // Don't want the ability to be fired when I am not in range\

            // TEMPORARY SOLUTION
            // if (ab.CheckRange)
            // {
            //     if (movement.CanMove)
            //     {
            //         Vector3 temp = transform.position;
            //         if (mouseTarget.Target == null)
            //         {
            //             // If the target does not exist , send the range somewh
            //             //temp = movement.MoveTarget;
            //             return;
            //         }
            //         else
            //             temp = mouseTarget.Target.transform.position;
            //         float dist = GetSquareDistance(transform.position, temp);
            //         //Debug.Log(dist + " from " + squareRange);
            //         if (dist > squareRange && movement.CanMove)
            //             return;
            //     }
            // }

            if (ab.CheckRange)  // If the player range should be checked when performing attacks
            {
                //EnemyControllerV2 nearest = AIManager.Instance.GetNearestEnemy(transform);
                // If the player IS NOT STOPPED (MOVING)
                if (movement.CanMove)
                {
                    float enemyDist = AIManager.Instance.GetShortestDistance(transform);
                    float destructDist = DropSystem.Instance.GetShortestDestructibleDistance(transform);
                    // Then check for the range to perform the attack, if the range is too far - eject
                    if (enemyDist > stats.Range.Value && destructDist > stats.Range.Value)
                        return;
                }

                //if (movement.CanMove)
                //{
                    // If the player can move, 
                //if (enemyDist > stats.Range.Value || destructDist > stats.Range.Value && !movement.isStopped)
                //{
                //    return;
                //}

                    // Check their range
                //}
                // If they cannot move, then allow them to perform the action, because of virtue of click allowed
                // If the enemy distance or destructible distance is far, and the player CAN MOVE
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
                ab.UseAbility(new AbilityEventArgs(ab, controller));      // Use the ability

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
            {
                if (PublicEventManager.Instance != null)
                {
                    PublicEventManager.Instance.FireOnCannot();
                }
            }
                Debug.Log("[AbilityHandler]: Cannot use: " + ab.Name);
        }
    }
    void AbilityEnded(Ability ab)
    {
        // Cooldowns will be activated based on key release - DO NOT WANT THIS TO HAPPEN

        if (ab != null)
        {
            held = false;                       // Key is not held anymore
            ab.EndAbility(new AbilityEventArgs(ab, controller));          // Call the abilities' end
            StopCoroutine("HandleHeld");        // Stop the handle held coroutine
            // ONLY start the cooldown if the ability CAN BE USED
            //Debug.Log("Ended");

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
                    ab.UseAbility(new AbilityEventArgs(ab, controller));          // Use the ability
                    canUseAbility[ab] = false;          // Flag usage
                    stats.ID_Bar.Value -= ab.Cost;      // Consume gauge
                    StartCoroutine(HandlePassive(ab as PassiveAbility));
                    StartCoroutine(HandleCooldown(ab));                 // Start handling the cooldown for this ability  
                    if (showDebug) Debug.Log("[AbilityHandler] Used: " + ab.Name);    // Output ability used
                }
            }
            if (PublicEventManager.Instance != null)
            {
                PublicEventManager.Instance.FireOnCannot();
            }
        }
    }
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
                    used.EndAbility(new AbilityEventArgs(used, controller));
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

        float currTime = used.Cooldown * (1 - stats.CooldownReduction.Value);   // CDR value is a decimal, reflected amount visually is * 10
        
        while (currTime > 0)
        {
            currTime -= Time.deltaTime;     // Countdownt the time
            used.CurrCooldown = currTime;
            yield return null;
        }

        canUseAbility[used] = true;                     // Allow usage again (time has passed)
    }
    #endregion
}