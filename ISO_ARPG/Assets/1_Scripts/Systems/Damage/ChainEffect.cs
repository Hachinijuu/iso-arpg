using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect : MonoBehaviour
{
    [SerializeField] Hitbox hitbox;

    [SerializeField] int chainLimit; // How much times the attack can chain
    // Chain attacks extend damage to the surroundings

    // How to apply chains?

    // This script will depend on an existing hitbox

    // When the hitbox successfully applies damage, the chain will extend the damage

    public void Awake()
    {
        hitbox.onDamageDealt += context => { HandleChainAttack(); };
    }

    private void HandleChainAttack()
    {
        // Do an overlap check or have a physics collider that this listens to

        // How do we handle chain attacks
        // Chain range, get the entity stats inside the chain range, make a chain and deal reduced damage to the target.

        // Hurtboxes require entity stats, so when finding, the two MUST be together, otherwise the implementation of the target is improper

        // There is a list of active enemies that can be referenced
    }
}
