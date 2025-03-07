using System.Collections;
using UnityEngine;

public struct DamageArgs
{
    public GameObject source;
    public float amount;
}

public class Hitbox : MonoBehaviour
{
    // HITBOX exists on weapons
    // - Source of damage
    #region VARIABLES
    [SerializeField] GameObject source; // The player / enemy owner of the hitbox, this is referenced so hits will not apply to their own hurtbox
    public GameObject Source { get { return source; } set { source = value; } }
    [SerializeField] float damage;    // THIS DAMAGE WILL BE CALCULATED AND APPLIED TO PEOPLE

    public bool ApplyDamage { get { return applyDamage; } set { applyDamage = value; } }
    private bool applyDamage = false;

    protected EntityStats stats; // -> hitbox, stats has abiliites -> apply these effects when I successfully hit
    // entity stats --> regen state, regen passive -> healthregen
    #endregion

    #region
    public delegate void DamageDealt(DamageArgs e);
    public event DamageDealt onDamageDealt;

    private void FireDamageDealt(DamageArgs e) { if (onDamageDealt != null) onDamageDealt(e); }
    #endregion
    #region UNITY FUNCTIONS


    // Shoudl the hitbox get a reference to the player, or be passed a reference to the relevant values when loaded --> pass a reference and then it set's it's own information
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void Start()
    {
        InitHitbox();
    }

    private void OnEnable()
    {
        InitHitbox();
    }
    private void OnTriggerEnter(Collider other)
    {
        // Only do detections if damage can be applied
        if (!applyDamage)
        {
            return;
        }

        // Refactor this hitbox hurtbox logic to not require other tags.

        // This is because, since the damage detection is on it's own layer
        // The damage interactions should only collide on this layer
        // Don't need to do tag matching or layer-masking since its done by the physics system

        // Hurtbox hb = other.gameObject.GetComponent<Hurtbox>();
        // if (hb != null)
        // {
        //     HandleCollision(hb);
        // }
        // else
        //     Debug.Log("[DamageSystem]: Hurtbox doesn't exist");

        if (other.CompareTag("Hurtbox") || other.CompareTag("Destructible"))
        {
           Hurtbox hb = other.GetComponent<Hurtbox>();
           //Debug.Log(other.name);
           if (hb)
           {
               HandleCollision(hb);
               Debug.Log("Handling collision");
           }
           else
               Debug.Log("[DamageSystem]: Hurtbox doesn't exist");
        }
    }
    #endregion

    #region FUNCTIONALITY
    public virtual void InitHitbox()
    {
        // Stops the hitbox from damaging the source of the damage
        // if (source != null)
        // {
        //     Collider sourceCollider = source.GetComponentInChildren<Collider>();
        //     Collider damageCollider = GetComponent<Collider>();

        //     // Want to ignore the collisions between the source of the damage and the hitbox
        //     if (sourceCollider != null && damageCollider != null)
        //     {
        //         //Debug.Log("Ignoring collisions between: " + sourceCollider.name + " and " + damageCollider.name);
        //         Physics.IgnoreCollision(sourceCollider, damageCollider);

        //         // THIS DOES NOT WORK ON PROJECTILES EVEN THOUGH REFERENCES ARE ASSIGNED PROPERLY, REPORT AS BUG BUT LEAVE FIXING UNTIL LATER
        //     }

        //     // Need to stop enemies from damaging each other once system is fleshed out
        // }

        // To avoid dealing damage to the one who sourced the damage
        // Look for their hitbox, get the collider off of that 

        if (source != null)
        {
            // Look for the hitbox on the source
            Hurtbox hb = source.GetComponent<Hurtbox>();
            if (hb)
            {
                Collider sourceCollider = hb.GetComponent<Collider>();
                Collider damageCollider = GetComponent<Collider>();
                if (sourceCollider != null && damageCollider != null)
                {
                    Physics.IgnoreCollision(sourceCollider, damageCollider);
                    Debug.Log("[Damage]: Ignoring collisions between " + sourceCollider.name + " and " + damageCollider.name);
                }
                else
                {
                    Debug.Log("[Damage]: No colliders found, not ignoring any collisions");
                }
            }
        }


        if (stats == null && source != null)
        {
            stats = source.GetComponent<EntityStats>();
        }

        if (stats != null)
        {
            Debug.Log("Assigned Damage Value");
            damage = stats.Damage.Value;    // Set the damage to the damage defined by the entity
        }

        // get the abilities from the stats

        // -> hit, regen for # of time w/o stack
        // empty time -> make complicated mana regen stacking
    }
    protected virtual void HandleCollision(Hurtbox hb)
    {
        // Before telling the hurtbox to take damage, do a crit roll to see if hitbox should take additional damage
        CritCalculation();
        DamageArgs args = new DamageArgs();
        args.amount = damage;
        hb.TakeDamage(damage);
        FireDamageDealt(args);
    }
    public void AllowDamageForTime(float window)
    {
        //Debug.Log("I want to allow damage for: " + window);
        StopAllCoroutines();
        StartCoroutine(DamageWindow(window));
    }

    protected virtual void CritCalculation()
    {
        if (stats is PlayerStats ps)
        {
            float critRoll = Random.Range(0.0f, 100.0f);
            if (critRoll <= ps.CritChance.Value)
            {
                damage += damage * ps.CritDamage.Value; // Additive damage
            } 
        }
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
