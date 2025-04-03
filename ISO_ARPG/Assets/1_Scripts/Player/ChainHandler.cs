using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChainHandler : MonoBehaviour
{
    // Reference to the prefab, this will exist on the player for easy accessibility
    PlayerController controller;
    PlayerStats stats;
    [SerializeField] GameObject chainPrefab;
    [SerializeField] static int baseChainAmount = 2;

    // Call this function whenever the chain effect should be created
    // Although, how should we know when to create a chain / handle a chain effect

    // When an enemy is hit, if the player / attack has additional chains on the attack, create the chain effect where the enemy was hit
    // The length of the effect is based on the arguments passed to it
    // I.E the default attack of ice spikes will apply chaining effects, so the given value will have a base of 1 chain amount, that is increased based on player chains
    public void CreateChainEffect(Vector3 pos, ChainEventArgs e)
    {
        ChainEventArgs args = e;
        args.chainAmount = baseChainAmount + e.chainAmount;     // The value given to this function will be the increased number of chains, this base chains should remain static
        ChainAttack.BuildChainAttack(chainPrefab, pos, args);
    }

    public void CreateChainFromHit(DamageArgs args)
    {
        Debug.Log("[ChainHandler]: Created Chain Attack");
        ChainEventArgs chainArgs = new ChainEventArgs();
        chainArgs.source = args.hit.GetComponent<Collider>();
        chainArgs.chainAmount = (int)stats.Chains.Value;
        chainArgs.chainID = 0;
        chainArgs.currChainStep = 0;
        CreateChainEffect(args.hit.transform.position, chainArgs);
        // Stop listening to this hitbox
        args.source.onDamageDealt -= CreateChainFromHit;
    }

    public void InitChainHandler()
    {
        stats = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
        AddAbilityListeners();
    }
    public void AddAbilityListeners()
    {
        foreach (Ability ab in stats.Abilities)
        {
            ab.onAbilityUsed += CheckShouldChain;
        }
    }

    public void CheckShouldChain(AbilityEventArgs e)
    {
        if (e.Hitboxes == null && e.Hitboxes.Count <= 0) { Debug.Log("[ChainHandler]: Missing Hurtboxes, Cannot listen."); return; } // If there are no hurtboxes passed to this function, then disregard completely, easy point of failure
        if (e.Ability is FrozenSpikes)
        {
            // This ability should chain by default
            foreach (Hitbox hb in e.Hitboxes)
            {
                hb.onDamageDealt += CreateChainFromHit;
            }
        }
        else
        {
            // These abilities will only chain if the amount of chains is greater than 0
            if (stats.Chains.Value <= 0) { return; }
            Debug.Log("Handling chains");
            // The ability can chain, add the listeners
            foreach (Hitbox hb in e.Hitboxes)
            {
                hb.onDamageDealt += CreateChainFromHit;
            }
        }
    }
}
