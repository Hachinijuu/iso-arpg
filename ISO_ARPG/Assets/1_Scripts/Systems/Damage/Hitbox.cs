using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public struct DamageArgs
{
    public Hitbox source;
    public float amount;
    public Hurtbox hit;
    public bool isCrit;
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
    //void Start()
    //{
    //
    //    InitHitbox();
    //}

    // Should the hitbox get a reference to the player, or be passed a reference to the relevant values when loaded --> pass a reference and then it set's it's own information
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    #endregion

    #region FUNCTIONALITY

    // Call this function when we need to check for hits

    public void InitHitbox(EntityStats stats)
    {
        this.stats = stats;
        if (stats != null)
        {
            damage = stats.Damage.Value;
        }
    }
    public virtual void HandleHits()
    {
        if (!applyDamage) { return; }   // If you cannot apply damage, return from this function
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, damageLayer); // Get what was collided with in a given space
        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject.CompareTag("Obstacle"))
            {
                HitBlock();
                return;
            }
            Hurtbox hb = hit.GetComponent<Hurtbox>();
            if (hb != null)
            {
                HandleCollision(hb);
            }
        }
    }

    public virtual void HitBlock()
    {
        Debug.Log("Early eject the hit");
    }

    public string[] hitblockers;

    // This is different from handle HITS, it will early return after the collision has been accounted for
    public virtual void HandleHit()
    {
        if (!applyDamage) { return; }   // If you cannot apply damage, return from this function
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, damageLayer); // Get what was collided with in a given space
        //Debug.Log(hitColliders.Length);
        foreach (Collider hit in hitColliders)
        {
            foreach (string s in hitblockers)
            {
                if (hit.gameObject.CompareTag(s))
                {
                    HitBlock();
                    return;
                }
            }

            //if (hit.gameObject.CompareTag("Obstacle"))
            //{
            //    HitBlock();
            //    return;
            //}
            //Debug.Log(hit.transform.name);
            Hurtbox hb = hit.GetComponent<Hurtbox>();
            if (hb != null)
            {
                //Debug.Log("Handling Collision");
                HandleCollision(hb);
                applyDamage = false;    // Stop allowing damage to pass through
                return; // Early return for only single hit detection
            }
        }
    }

    public void Attack()
    {
        applyDamage = true;
        HandleHit();            // This is ONLY for single target attack
        applyDamage = false;
    }

    public void AttackForTime(float duration)
    {
        applyDamage = true;
        AllowDamageForTime(duration);
    }

    public DamageArgs GetArgs(Hurtbox hb)
    {
        DamageArgs args = new DamageArgs();
        args.hit = hb;
        args.amount = damage;
        args.source = this;
        return args;
    }
    protected virtual void HandleCollision(Hurtbox hb)
    {
        // Before telling the hurtbox to take damage, do a crit roll to see if hitbox should take additional damage
        if (!hb.Stats.isDead)
        {
            if (hb.GetComponent<AttackMarker>() != null) { return; }
            hb.AddComponent<AttackMarker>();
            DamageArgs args = GetArgs(hb);
            CritCalculation(ref args);
            hb.TakeDamage(args);
            Debug.Log("[HitboxSource]: Dealt " + args.amount);
            FireDamageDealt(args);
        }

    }
    protected virtual void CritCalculation(ref DamageArgs args)
    {
        if (stats is PlayerStats ps)
        {
            float critRoll = Random.Range(0.0f, 100.0f);
            if (critRoll <= ps.CritChance.Value)
            {
                damage += ps.CritDamage.Value; // Tweaked calc 
                
                //* (1 + (ps.CritDamage.Value / 100)); // Additive damage
                args.isCrit = true;
            } 
        }
    }

    public void AllowDamageForTime(float window)
    {
        //Debug.Log("I want to allow damage for: " + window);
        //Debug.Log("Got into damage");
        StopAllCoroutines();
        StartCoroutine(HitWindow());            // This will detect the hits
        StartCoroutine(DamageWindow(window));   // This will determine how long the hits are allowed
    }

    public void AllowDamageForTime(float window, bool value)
    {
        //Debug.Log("I want to allow damage for: " + window);
        //Debug.Log("Got into damage");
        StopAllCoroutines();
        StartCoroutine(HitWindow(value));       // This will detect the hits
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
        { 
            StartCoroutine(HitWindow(true));
        }
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

    IEnumerator HitWindow(bool continous)
    {
        do 
        {
            HandleHits();
            yield return null;
        } while (applyDamage);
    }

    #endregion
    #region DEBUG
    public bool debugDraw = false;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (applyDamage)
        {
            Gizmos.DrawSphere(transform.position, attackRange);
            //Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
        }
        if (debugDraw)
        {
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
    #endregion
}
