using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
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
        input.actions["Ab1"].started += context => { AbilityBegan(0); held = true; };
        input.actions["Ab1"].canceled += context => { AbilityEnded(0); held = false; };
        input.actions["Ab2"].started += context => { AbilityBegan(1); held = true; };
        input.actions["Ab2"].canceled += context => { AbilityEnded(1); held = false; };
        input.actions["IDab"].performed += context => { UseIdentityAbility(); };
    }

    void UnmapPlayerActions()
    {
        input.actions["Ab1"].started -= context => { AbilityBegan(0); };
        input.actions["Ab1"].canceled -= context => { AbilityEnded(0);};
        input.actions["Ab2"].started -= context => { AbilityBegan(1); };
        input.actions["Ab2"].canceled -= context => { AbilityEnded(1); };
        input.actions["IDab"].performed -= context => { UseIdentityAbility(); };
    }


    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInput>();

        MapPlayerActions();

        if (stats != null)
        {
            if (stats.Class.abilities.Count > 0)
            {
                InitAbilities();
            }
            else
                Debug.LogWarning("[AbilityHandler]: No abilities to read from");

        }
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

    void AbilityBegan(int index)
    {
        if (stats != null)                                                          // If player stats exists
        {
            if (stats.Class.abilities != null && stats.Class.abilities.Count > 0)   // If the class has abilities assigned to it
            {
                if (index < stats.Class.abilities.Count)                            // If the requested index is within the list
                {
                    Ability ab = stats.Class.abilities[index];  // Get a reference to the ability at the provided index

                    // Check if the ability is not on cooldown
                    if (canUseAbility[ab])
                    {
                        // Check if the player has enough mana to use the ability
                        if ((stats.Mana.Value - ab.Cost) < 0) // If consumed mana resultes in less, not enough mana
                        {
                            Debug.Log("[AbilityHandler]: Not enough mana to use " + ab.Name + "(" + stats.Mana.Value + " / " + ab.Cost + ")");
                        }
                        else    // Player has enough mana to use the ability
                        {
                            ab.UseAbility(gameObject);          // Use the ability
                            stats.Mana.Value -= ab.Cost;        // Consume the cost of mana
                            //if (ab.StopsMovement) // need to have a reference to the player controller to do agent stopping
                            //    
                            if (!ab.Channelable)                // If the ability is not channelable
                                AbilityEnded(index);            // End the ability
                            else                                // Ability is channelable (can hold)
                                StartCoroutine(HandleHeld(index, ab));
                            if (showDebug) Debug.Log("[AbilityHandler] Used: " + ab.Name);
                        }
                    }
                    else
                        Debug.Log("[AbilityHandler]: " + ab.Name + " on cooldown");
                }
                else
                    Debug.LogWarning("[AbilityHandler]: Requested abilitiy index: " + index + " is out of range");
            }
            else
                Debug.LogWarning("[AbilityHandler]: Class has no abilities");
        }
        else
            Debug.LogWarning("[AbilityHandler]: Player has no stats to read abilities from!");
    }

    void AbilityEnded(int index)
    {
        held = false;
        if (stats != null)                                                          // If player stats exists
        {
            if (stats.Class.abilities != null && stats.Class.abilities.Count > 0)   // If the class has abilities assigned to it
            {
                if (index < stats.Class.abilities.Count)                            // If the requested index is within the list
                {
                    Ability ab = stats.Class.abilities[index];  // Get a reference to the ability at the provided index
                    canUseAbility[ab] = false;          // Flag usage
                    StartCoroutine(HandleCooldown(ab)); // Start handling the cooldown for this ability
                    if (showDebug) Debug.Log("[AbilityHandler]: " + ab.Name + " ended");
                }
                else
                    Debug.LogWarning("[AbilityHandler]: Requested abilitiy index: " + index + " is out of range");
            }
            else
                Debug.LogWarning("[AbilityHandler]: Class has no abilities");
        }
        else
            Debug.LogWarning("[AbilityHandler]: Player has no stats to read abilities from!");
    }

    void UseIdentityAbility()
    {
        if (stats != null)
        {
            if (stats.Class.identityAbility != null)
            {
                Ability ab = stats.Class.identityAbility;
                if (canUseAbility[ab])  // Check if the ability is not on cooldown
                {
                    if (stats.ID_Bar.Value == stats.ID_Bar.MaxValue)            // Check if there is enough of the gauge built up
                    {
                        ab.UseAbility(gameObject);                              // Use the ability
                        canUseAbility[ab] = false;                              // Flag usage
                        stats.ID_Bar.Value -= ab.Cost;                          // Consume gauge
                        HandleCooldown(ab);                                     // Start handling the cooldown for this ability
                        if (showDebug) Debug.Log("[AbilityHandler] Used: " + ab.Name);
                    }
                }

            }
        }
    }

    IEnumerator HandleHeld(int index, Ability used)
    {
        // Channeled abilities will consume cost every n seconds
        // This will be defined by the abilities interval
        do
        {
            if (showDebug) Debug.Log("[AbilityHandler] Using " + used.Name);
            yield return new WaitForSeconds(used.Interval);
            // Once the time has passed
            if ((stats.Mana.Value - used.Cost) < 0) // If the consumed mana would result in no remaining mana
            {
                AbilityEnded(index);                    // End the ability
            }
            else    // Remaining mana, consume it.
            {
                stats.Mana.Value -= used.Cost;          // Consume the cost of mana
            }
        } while (held);

        //while (held)
        //{
        //    Debug.Log("[AbilityHandler] Using " + used.Name);
        //    yield return new WaitForSeconds(used.Interval);
        //    // Once the time has passed
        //    if ((stats.Mana.Value - used.Cost) < 0) // If the consumed mana would result in no remaining mana
        //    {
        //        AbilityEnded(index);                    // End the ability
        //    }
        //    else    // Remaining mana, consume it.
        //    {
        //        stats.Mana.Value -= used.Cost;          // Consume the cost of mana
        //    }
        //}
    }

    IEnumerator HandleCooldown(Ability used)
    {
        yield return new WaitForSeconds(used.Cooldown); // Wait for the cooldown time
        canUseAbility[used] = true;                     // Allow usage again (time has passed)
    }
}
