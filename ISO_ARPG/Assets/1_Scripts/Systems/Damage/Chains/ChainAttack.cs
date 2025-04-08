using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChainAttack : MonoBehaviour
{
// What does this chain need to know
    // It needs to know it's current step
    // It needs to know it's id
    // It needs to know how far it can chain from itself
    public static float chainDamageReduction = 0.25f;
    public LayerMask detectionLayer;
    [SerializeField] float chainRange = 2.5f;
    public int chainId;
    public int chainStep;    
    public List<Collider> validTargets = new List<Collider>();
    ChainEventArgs args;
    public static ChainAttack BuildChainAttack(GameObject prefab, Vector3 pos, ChainEventArgs e)
    {
        GameObject go = Instantiate(prefab, pos, Quaternion.identity);  // Get a chain attack at the position
        ChainAttack attack = go.GetComponent<ChainAttack>();        // Get the chain attack component
        CombatFeedbackManager.Instance.PlayChainParticles(pos);

        attack.InitChain(e);
        attack.HandleAttack(e);                                           // Tell the attack to handle the chain
        return attack;
    }
    public void InitChain(ChainEventArgs e)
    {
        // Set the values of this script to match the arguments
        // As the values are passed along, the arguments will update accordingly(?)
        args = e;
        // if (e.chainID == 0)
        // {
        //     chainId++;
        // }

        e.source = this.GetComponent<Collider>();
        chainId = args.chainID;
        chainStep = e.currChainStep +1;
        //chainId = args.chainID;
    }

    private void OnEnable() 
    {
        InitChain(args);
        HandleAttack(args);
        Destroy(gameObject, 0.25f);
    }
    public void HandleAttack(ChainEventArgs e)
    {
        StartCoroutine(HandleChain(e));
    }
    public IEnumerator HandleChain(ChainEventArgs e)
    {
        // Check if this chain has reached its max length
        if (chainStep > e.chainAmount) { yield break; }

        GetValidTargets(e);

        if (validTargets.Count > 0)
        {
            foreach (Collider target in validTargets)
            {
                Hurtbox hb = target.GetComponent<Hurtbox>();
                if (hb == null) { yield break; }    // If the hurtbox doesn't exist, ignore

                yield return new WaitForSeconds(0.1f);

                // Handle the damage here;
                DamageArgs args = e.damageArgs;
                args.amount *= (1 - chainDamageReduction * chainStep);
                args.hit = hb;
                //args.hit = hb;

                //float damage = e.damage * ( 1 - chainDamageReduction * chainStep);
                hb.TakeDamage(args);    // Reduced by the base damage value and the chainStep
                
                
                // Only extend the chain if less than the current step is less than the chain amount
                if (chainStep < e.chainAmount)
                {
                    //e.chainID = chainId;
                    e.source = target;
                    e.currChainStep = chainStep;
                    BuildChainAttack(this.gameObject, target.transform.position, e);
                }
            }
        }
    }
    public void GetValidTargets(ChainEventArgs e)
    {
        validTargets.Clear();
        Collider[] hitTargets = Physics.OverlapSphere(transform.position, chainRange, detectionLayer);
        //int idCounter = 0;
        foreach (Collider c in hitTargets)
        {
            if (c != e.source)  // If the hit target is NOT the source hit
            {
                Hurtbox hb = c.GetComponent<Hurtbox>();
                if (hb == null || hb.Stats.isDead) { continue; }    // If the hitbox doesn't exist, or is dead, then skip that for the valid target selection
                //if (validTargets.Count == e.chainSpread) { return; }
                if (e.chainID == 0 && e.chainID <= e.chainSpread)
                {
                    chainId++;
                }
                ChainHitMarker marker = c.GetComponent<ChainHitMarker>();
                if (marker == null) // If a marker does not exist
                {
                    marker = c.AddComponent<ChainHitMarker>(); 
                    marker.chainID = chainId;   // Let whoever was hit, know they were hit by this chainID
                    validTargets.Add(c);
                }
                else
                {
                    // Marker does exist, compare to ID
                    if (marker.chainID == chainId) { continue; }    // They have matching ID's, ignore the hit
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        switch(chainStep)
        {
            case 0:
            Gizmos.color = Color.red;
            break;
            case 1:
            Gizmos.color = Color.yellow;
            break;
            case 2:
            Gizmos.color = Color.green;
            break;
            case 3:
            Gizmos.color = Color.cyan;
            break;
            case 4:
            Gizmos.color = Color.blue;
            break;
            case 5:
            Gizmos.color = Color.magenta;
            break;
        }
        Gizmos.DrawWireSphere(transform.position, chainRange);
    }
}

public struct ChainEventArgs
{
    public Collider source;
    public DamageArgs damageArgs;
    public int chainID;
    public int currChainStep;
    public int chainAmount;
    public int chainSpread;
}
