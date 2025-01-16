using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // HITBOX exists on weapons
    // - Source of damage
    [SerializeField] GameObject source; // The player / enemy owner of the hitbox, this is referenced so hits will not apply to their own hurtbox
    [SerializeField] int damage;


    public bool ApplyDamage { get { return applyDamage; } set { applyDamage = value; } }
    private bool applyDamage = false;

    private void Start()
    {
        Collider sourceCollider = source.GetComponent<Collider>();
        Collider damageCollider = GetComponent<Collider>();

        // Want to ignore the collisions between the source of the damage and the hitbox
        if (sourceCollider != null && damageCollider != null)
        {
            Physics.IgnoreCollision(sourceCollider, damageCollider);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // Only do detections if damage can be applied
        if (!applyDamage)
        {
            return;
        }

        if (other.CompareTag("Hurtbox"))
        { 
            Hurtbox hb = other.GetComponent<Hurtbox>();
            if (hb != null)
            {
                hb.TakeDamage(damage);
            }
            else
                Debug.Log("[DamageSystem]: Hurtbox doesn't exist");
        }
    }
}
