using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // HITBOX exists on weapons
    // - Source of damage
    #region VARIABLES
    [SerializeField] GameObject source; // The player / enemy owner of the hitbox, this is referenced so hits will not apply to their own hurtbox
    [SerializeField] float damage;    // THIS DAMAGE WILL BE CALCULATED AND APPLIED TO PEOPLE

    public bool ApplyDamage { get { return applyDamage; } set { applyDamage = value; } }
    private bool applyDamage = false;
    #endregion
    #region UNITY FUNCTIONS


    // Shoudl the hitbox get a reference to the player, or be passed a reference to the relevant values when loaded --> pass a reference and then it set's it's own information
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void Start()
    {
        if (source != null)
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

    }
    private void OnTriggerEnter(Collider other)
    {
        // Only do detections if damage can be applied
        if (!applyDamage)
        {
            return;
        }
        HandleCollision(other);
    }
    #endregion

    #region FUNCTIONALITY
    protected virtual void HandleCollision(Collider other)
    {
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
    public void AllowDamageForTime(float window)
    {
        //Debug.Log("I want to allow damage for: " + window);
        StopAllCoroutines();
        StartCoroutine(DamageWindow(window));
    }

    protected virtual void StartDamageWindow()
    {
        applyDamage = true;
    }

    protected virtual void EndDamageWindow()
    {
        applyDamage = false;
    }

    IEnumerator DamageWindow(float time)
    {
        StartDamageWindow();
        yield return new WaitForSeconds(time);
        EndDamageWindow();
    }
    #endregion
    #region DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (applyDamage)
            Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
    #endregion
}
