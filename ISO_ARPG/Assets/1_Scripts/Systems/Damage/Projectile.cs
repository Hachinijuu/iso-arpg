using System.Collections;
using UnityEngine;

public class Projectile : Hitbox
{
    // A projectiles IS A hitbox with some extended functionality.

    // It is expected to exist for some time, OR, recycle itself once a hit has been made.

    #region VARIABLES
    [SerializeField] Rigidbody rb;

    [Header("Projectile Settings")]
    [SerializeField] bool pierces;
    [SerializeField] public float uptime;
    [SerializeField] float speed;
    [SerializeField] bool destroyAfterDone;
    public int pierceLimit = 4; // Set this value for the things, will only use if piercing is active;

    int pierceCounter = 0;

    #endregion
    #region FUNCTIONALITY
    // public override void HandleHit()
    // {
    //     if (!applyDamage) { return; }   // If you cannot apply damage, return from this function
    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, damageLayer); // Get what was collided with in a given space
    //     foreach (Collider hit in hitColliders)
    //     {
    //         // if what was hit is an obstacle
    //         if (hit.CompareTag("Obstacle")) { gameObject.SetActive(false); return; } // Eject, and recycle the object
    //         Hurtbox hb = hit.GetComponent<Hurtbox>();
    //         if (hb != null)
    //         {
    //             HandleCollision(hb);
    //         }
    //     }
    // }

    // public override void HandleHits()
    // {
    //     if (!applyDamage) { return; }   // If you cannot apply damage, return from this function
    //     Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, damageLayer); // Get what was collided with in a given space
    //     //Debug.Log(hitColliders.Length);
    //     foreach (Collider hit in hitColliders)
    //     {
    //         if (hit.CompareTag("Obstacle")) { gameObject.SetActive(false); return; } // Eject, and recycle the object
    //         Hurtbox hb = hit.GetComponent<Hurtbox>();
    //         if (hb != null)
    //         {
    //             //Debug.Log("Handling Collision");
    //             HandleCollision(hb);
    //             applyDamage = false;    // Stop allowing damage to pass through
    //             return; // Early return for only single hit detection
    //         }
    //     }
    // }

    public override void HitBlock()
    {
        if (destroyAfterDone)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    protected override void HandleCollision(Hurtbox hb)
    {
        base.HandleCollision(hb);   // apply damage

        if (!pierces)               // if it does not pierece
            gameObject.SetActive(false);    // deactivate

        // it does pierce, handle the counter
        if (pierces)
        {
            pierceCounter++;
            if (pierceCounter == pierceLimit)
            {
                gameObject.SetActive(false);
            }
        }

        if (destroyAfterDone)
            Destroy(gameObject);
    }
    public void FireProjectile()
    {
        // Apply force to this object in the forward direction
        if (pierces)
        {
            AllowDamageForTime(uptime, true);
        }
        else
        {
            AllowDamageForTime(uptime);
        }
        // Move the object
        //rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        //StopAllCoroutines();
        StartCoroutine(ProjectileMotion());
    }

    public void FireAimedProjectile(Vector3 target)
    {
        // Apply force to this object in the forward direction
        Vector3 dir = (target - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        if (pierces)
        {
            AllowDamageForTime(uptime, true);
        }
        else
        {
            AllowDamageForTime(uptime);
        }
        // Move the object
        //rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        //StopAllCoroutines();
        StartCoroutine(ProjectileMotion());
    }

    IEnumerator ProjectileMotion()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            transform.position += (transform.forward * speed * Time.deltaTime);
        }
    }
    protected override void EndDamageWindow()
    {
        base.EndDamageWindow();
        if (destroyAfterDone)
            Destroy(gameObject);
        gameObject.SetActive(false);
        //Debug.Log("Uptime ended");
    }
    #endregion
    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(20, 120, 100, 20), "Shoot"))
    //    {
    //        Debug.Log("Shot a projectile");
    //        FireProjectile();
    //    }
    //}
}
