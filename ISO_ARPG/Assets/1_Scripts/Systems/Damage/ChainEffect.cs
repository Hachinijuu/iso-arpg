using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect : MonoBehaviour
{
    [SerializeField] Hitbox hitbox;

    [SerializeField] int chainLimit; // How much times the attack can chain
    public float chainRange = 2.5f;
    // Chain attacks extend damage to the surroundings

    public LayerMask detectionLayer;
    private int chainAmount = 0;

    // How to apply chains?

    // This script will depend on an existing hitbox

    // When the hitbox successfully applies damage, the chain will extend the damage

    public void Awake()
    {
        hitbox.onDamageDealt += context => { HandleChainAttack(context, ref chainAmount, chainLimit); };   // When the hitbox deals damage to a target
    }
    private void HandleChainAttack(DamageArgs args, ref int chainAmount, int chainLimit)
    {
        if (chainAmount >= chainLimit)
        {
            chainAmount = 0;
            return;
        }

        Hurtbox hit = args.hit;

        // hit is whoever was hit by the attack
        // build a physics collider around the hit target to get the subsequent targets to apply damage to
        Debug.Log("[Damage]: Searching for chain targets");
        Collider[] chainTargets = Physics.OverlapSphere(transform.position, chainRange, detectionLayer);

        if (chainTargets.Length > 0 && chainTargets != null)
        {
            Debug.Log("[Damage]: " + chainTargets.Length + " Chain targets exist");

            // If someone is within the chain range
            // Apply the chained damage to them and have THEM chain

            Hurtbox chainedHit = null;

            // Hit the target with reduced damage
            foreach (Collider collider in chainTargets)
            {

                // Need to ignore the source of the damage
                // Can ignore the player completely, or try to rework to avoid only the damage source

                // Damage source avoidance needs to be reworked for projectile functionality killing the player
                if (collider.CompareTag("Hurtbox") && !collider.transform.parent.CompareTag("Player")) // The hurtbox hit in the chain attack
                { 
                    chainedHit = collider.GetComponent<Hurtbox>();
                    Debug.Log(chainedHit.transform.parent.name);
                    break;
                }
            }


            //Hurtbox chainedHit = chainTargets[0].transform.GetComponent<Hurtbox>();
            if (chainedHit != null)
            {
                Debug.Log("[Damage]: Chained Attack");
                chainAmount += 1;

                // 25% reduced damage per chain sequence
                // total amount to take * (0.25 * chain step) // 2 steps = 50% of total damage
                float chainDamage = args.amount * (0.25f * chainAmount);    // Reduction per chain sequence
                chainedHit.TakeDamage(chainDamage); // Have chain attacks as a recursive function with a limiter on the amount via the chain limit
                DamageArgs chainArgs = new DamageArgs();
                chainArgs.amount = chainDamage;
                chainArgs.hit = chainedHit;
                HandleChainAttack(chainArgs, ref chainAmount, chainLimit);
                //ContinueChain(ref chainAmount, chainLimit);
                return;
            }
        }

        // Need to know who was hit by the hitbox

        // Do an overlap check or have a physics collider that this listens to

        // How do we handle chain attacks
        // Chain range, get the entity stats inside the chain range, make a chain and deal reduced damage to the target.

        // Hurtboxes require entity stats, so when finding, the two MUST be together, otherwise the implementation of the target is improper

        // There is a list of active enemies that can be referenced
    }

    private void ContinueChain(ref int currChain, int chainLimit)
    {
        if (currChain >= chainLimit)
            currChain = 0;
            return;

        // Handle the chain and return

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chainRange);
    }
}
