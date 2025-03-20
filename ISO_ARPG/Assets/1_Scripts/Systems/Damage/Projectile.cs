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
    [SerializeField] float uptime;
    [SerializeField] float speed;

    #endregion
    #region FUNCTIONALITY
    protected override void HandleCollision(Hurtbox hb)
    {
        base.HandleCollision(hb);   // apply damage
        if (!pierces)               // if it does not pierece
            gameObject.SetActive(false);    // deactivate
    }
    public void FireProjectile()
    {
        // Apply force to this object in the forward direction
        AllowDamageForTime(uptime);
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
