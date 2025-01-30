using System.Collections;
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

        // Need to stop enemies from damaging each other once system is fleshed out
    }

    public void AllowDamageForTime(float window)
    {
        //Debug.Log("I want to allow damage for: " + window);
        StopAllCoroutines();
        StartCoroutine(DamageWindow(window));
    }

    IEnumerator DamageWindow(float time)
    {
        applyDamage = true;
        yield return new WaitForSeconds(time);
        applyDamage = false;
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
            //Debug.Log("HURTBOX FOUND");
            Hurtbox hb = other.GetComponent<Hurtbox>();
            if (hb != null)
            {
                hb.TakeDamage(damage);
            }
            else
                Debug.Log("[DamageSystem]: Hurtbox doesn't exist");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (applyDamage)
            Gizmos.DrawCube(transform.position, new Vector3(1,1,1));
    }
}
