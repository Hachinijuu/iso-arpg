using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : Projectile
{
    public float explosionRadius = 2.0f;
    public LayerMask detectionLayer;
    protected override void HandleCollision(Hurtbox hb)
    {
        // When handling the collision (Target has been hit, create an explosion here)
        Hurtbox hit = hb;

        // hit is whoever was hit by the attack
        // build a physics collider around the hit target to get the subsequent targets to apply damage to
        Debug.Log("[Damage]: Searching for chain targets");
        Collider[] explosionHit = Physics.OverlapSphere(transform.position, explosionRadius, detectionLayer);

        if (explosionHit.Length > 0 && explosionHit != null)
        {
            foreach (Collider collider in explosionHit)
            {
                if (collider.CompareTag("Hurtbox") && !collider.transform.parent.CompareTag("Player")) // The hurtbox hit by the explosion
                { 
                    Hurtbox explodeHB = collider.GetComponent<Hurtbox>();
                    explodeHB.TakeDamage(damage);
                    break;
                }
            }
        }

        base.HandleCollision(hb);
    }
}
