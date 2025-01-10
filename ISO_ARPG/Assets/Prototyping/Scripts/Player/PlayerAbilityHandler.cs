using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityHandler : MonoBehaviour
{
    [SerializeField] bool showDebug = false;
    PlayerStats stats;
    PlayerInput input;

    Dictionary<Ability, bool> canUseAbility = new Dictionary<Ability, bool>();
    bool held;

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
        for (int i = 1; i <= stats.Class.abilities.Count; i++)
        {
            string keyName = "Ab" + i;
            Ability ab = stats.Class.abilities[i - 1];
            if (ab is ChannelAbility)
            {
                input.actions[keyName].started += context => { AbilityBegan(ab); };
                input.actions[keyName].canceled += context => { AbilityEnded(ab); };
            }
            else
                input.actions[keyName].performed += context => { AbilityBegan(ab); };
        }
        input.actions["IDab"].performed += context => { UseIdentityAbility(stats.Class.identityAbility); };
    }

    void UnmapPlayerActions()
    {
        for (int i = 1; i <= stats.Class.abilities.Count; i++)
        {
            string keyName = "Ab" + i;
            Ability ab = stats.Class.abilities[i - 1];
            if (ab is ChannelAbility)
            {
                input.actions[keyName].started -= context => { AbilityBegan(ab); };
                input.actions[keyName].canceled -= context => { AbilityEnded(ab); };
            }
            else
                input.actions[keyName].performed -= context => { AbilityBegan(ab); };
        }
        input.actions["IDab"].performed -= context => { UseIdentityAbility(stats.Class.identityAbility); };
    }


    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInput>();


        if (stats != null)
        {
            if (stats.Class.abilities.Count > 0)
            {
                InitAbilities();
            }
            else
                Debug.LogWarning("[AbilityHandler]: No abilities to read from");
        }
        // ORDER MATTERS
        // Abilities must be initialized before mapping the inputs
        // This is because the functions that are mapped, respond to the types of abilities that are loaded.

        MapPlayerActions();
    }

    private void OnDestroy()
    {
        UnmapPlayerActions();
    }
    public void InitAbilities()
    {
        foreach (Ability ab in stats.Class.abilities)
        {
            canUseAbility.Add(ab, true);
        }
        canUseAbility.Add(stats.Class.identityAbility, true);
    }

    void AbilityBegan(Ability ab)
    {
        if (ab != null)
        {
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
                    StartCoroutine(HandleHeld(channelAb));  // Handle the consumption of the hold
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
        if (ab != null)
        {
            held = false;                       // Key is not held anymore
            ab.EndAbility(gameObject);          // Call the abilities' end
            StopCoroutine("HandleHeld");        // Stop the handle held coroutine
            // ONLY start the cooldown if the ability CAN BE USED

            if (!canUseAbility[ab])
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
                    HandleCooldown(ab);                 // Start handling the cooldown for this ability  
                    if (showDebug) Debug.Log("[AbilityHandler] Used: " + ab.Name);    // Output ability used
                }

            }
        }
    }

    // This function exists because value assignments ContextCallbacks is not very reliable (Only exists within the frame of the callback)

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
                stats.Mana.Value -= used.Cost;          // Consume the cost of mana
            }
        } while (held);
    }

    IEnumerator HandleCooldown(Ability used)
    {
        // With this wrapper functionality, PlayerAbilityHandler do calculations here (if CDR) and have updated cooldown values.
        // If theres a reference to the HUD controller, then the HUD's cooldown can also be called here for 1:1 timer

        yield return new WaitForSeconds(used.Cooldown); // Wait for the cooldown time
        canUseAbility[used] = true;                     // Allow usage again (time has passed)
    }
}


// Ability System can be refactored to make use of bool events...
// IsPressed
// WasPressedThisFrame
// WasReleasedThisFrame

// BUT - since context callbacks are already setup, that is probably the way to go about it.
// Need to handle active event, NOT call CD reset if the cooldown is already ticking...