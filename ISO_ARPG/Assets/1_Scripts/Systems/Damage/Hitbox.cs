using System.Collections;
using UnityEngine;

public struct DamageArgs
{
    public GameObject source;
    public float amount;
    public Hurtbox hit;
}

public class Hitbox : MonoBehaviour
{
    // HITBOX exists on weapons
    // - Source of damage
    #region VARIABLES
    [SerializeField] GameObject source; // The player / enemy owner of the hitbox, this is referenced so hits will not apply to their own hurtbox
    public GameObject Source { get { return source; } set { source = value; } }
    [SerializeField] protected float damage;    // THIS DAMAGE WILL BE CALCULATED AND APPLIED TO PEOPLE
    [SerializeField] protected float attackRange;
    public LayerMask damageLayer;
    public bool ApplyDamage { get { return applyDamage; } set { applyDamage = value; } }
    protected bool applyDamage = false;

    protected EntityStats stats; // -> hitbox, stats has abiliites -> apply these effects when I successfully hit
    // entity stats --> regen state, regen passive -> healthregen
    #endregion

    #region
    public delegate void DamageDealt(DamageArgs e);
    public event DamageDealt onDamageDealt;

    private void FireDamageDealt(DamageArgs e) { if (onDamageDealt != null) onDamageDealt(e); }
    #endregion
    #region UNITY FUNCTIONS

    // Should the hitbox get a reference to the player, or be passed a reference to the relevant values when loaded --> pass a reference and then it set's it's own information
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    #endregion

    #region FUNCTIONALITY

    // Call this function when we need to check for hits
    public void HandleHits()
    {
        if (!applyDamage) { return; }   // If you cannot apply damage, return from this function
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, damageLayer); // Get what was collided with in a given space
        foreach (Collider hit in hitColliders)
        {
            Hurtbox hb = hit.GetComponent<Hurtbox>();
            if (hb != null)
            {
                HandleCollision(hb);
            }
        }
    }

    // This is different from handle HITS, it will early return after the collision has been accounted for
    public void HandleHit()
    {
        if (!applyDamage) { return; }   // If you cannot apply damage, return from this function
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, damageLayer); // Get what was collided with in a given space
        foreach (Collider hit in hitColliders)
        {
            Hurtbox hb = hit.GetComponent<Hurtbox>();
            if (hb != null)
            {
                HandleCollision(hb);
                return; // Early return for only single hit detection
            }
        }
    }
    protected virtual void HandleCollision(Hurtbox hb)
    {
        // Before telling the hurtbox to take damage, do a crit roll to see if hitbox should take additional damage
        CritCalculation();
        DamageArgs args = new DamageArgs();
        args.amount = damage;
        args.source = source;
        hb.TakeDamage(damage);

        // Give the current stats an amount of mana from the hit
        // The problem is -> if I hit multiple targets, do I get mana for EACH target hit, or that I hit a target?

        FireDamageDealt(args);
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

    public void AllowDamageForTime(float window)
    {
        //Debug.Log("I want to allow damage for: " + window);
        StopAllCoroutines();
        StartCoroutine(HitWindow());            // This will detect the hits
        StartCoroutine(DamageWindow(window));   // This will determine how long the hits are allowed
    }
    protected virtual void StartDamageWindow()
    {
        applyDamage = true;
    }

    protected virtual void EndDamageWindow()
    {
        applyDamage = false;
    }
    
    public void OpenHitWindow()
    {
        if (applyDamage)
        StartCoroutine(HitWindow());
    }

    IEnumerator DamageWindow(float time)
    {
        StartDamageWindow();
        yield return new WaitForSeconds(time);
        EndDamageWindow();
    }

    IEnumerator HitWindow()
    {
        do
        {
            HandleHit();        // By default, only look for single target per frame
            yield return null;
        } while (applyDamage);
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
