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
    protected override void HandleCollision(Collider other)
    {
        Debug.Log("Hit something");
        base.HandleCollision(other);
        if (other.CompareTag("Hurtbox"))
        {
            if (!pierces)
                gameObject.SetActive(false);
        }
    }
    public void FireProjectile()
    {
        // Apply force to this object in the forward direction
        AllowDamageForTime(uptime);
        // Move the object
        rb.AddForce(Vector3.forward * speed, ForceMode.VelocityChange);
    }
    protected override void EndDamageWindow()
    {
        base.EndDamageWindow();
        gameObject.SetActive(false);
        Debug.Log("Uptime ended");
    }
    #endregion
    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 120, 100, 20), "Shoot"))
        {
            Debug.Log("Shot a projectile");
            FireProjectile();
        }
    }
}
