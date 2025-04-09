using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : Projectile
{
    [SerializeField] protected GameObject destructionEffect;
    public float explosionRadius = 2.0f;
    public float particleTime = 0.25f;
    public LayerMask detectionLayer;
    protected override void HandleCollision(Hurtbox hb)
    {
        // When handling the collision (Target has been hit, create an explosion here)
        Hurtbox hit = hb;

        // hit is whoever was hit by the attack
        // build a physics collider around the hit target to get the subsequent targets to apply damage to
        Debug.Log("[Damage]: Searching for explosion hit targets");
        Collider[] explosionHit = Physics.OverlapSphere(transform.position, explosionRadius, detectionLayer);

        if (explosionHit.Length > 0 && explosionHit != null)
        {
            foreach (Collider collider in explosionHit)
            {
                if (collider.CompareTag("Hurtbox") && !collider.transform.parent.CompareTag("Player")) // The hurtbox hit by the explosion
                { 
                    Hurtbox explodeHB = collider.GetComponent<Hurtbox>();
                    DamageArgs args = GetArgs(hb);
                    explodeHB.TakeDamage(args);
                    
                    // Early breaking gets rid of the splash damage, hit each target that was within the AOE radius
                    //break;
                }
            }
        }
        GameObject effect = Instantiate(destructionEffect);
        effect.transform.position = transform.position;

        // Shut off functionality, but leave lag for things to happen, then shutoff

        Destroy(effect, particleTime);

        base.HandleCollision(hb);
    }
}
