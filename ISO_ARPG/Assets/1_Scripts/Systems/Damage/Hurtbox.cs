using System.Collections;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    #region VARIABLES
    // THIS IS NOT FINAL IMPLEMENTATION
    // IT WILL BE FLESHED ONCE STAT LOADING AND CHARACTER INFORMATION IS ORGANIZED

    // Get a reference to the health on the specified object
    public EntityStats Stats { get { return stats; } }
    [SerializeField] protected EntityStats stats;
    [SerializeField] public bool applyKnockback;
    #endregion
    //TrackedStat health;

    #region EVENTS
    // Fire event whenever this hurtbox takes damage
    public delegate void OnDamaged(float value);
    public OnDamaged onDamaged;
    private void FireOnDamaged(float value) { if (onDamaged != null) { onDamaged(value); } }

    //Rigidbody rb;
    #endregion
    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("[DamageSystem]: Missing stat reference");
        }
        if (transform.CompareTag("Destructible"))
        {
            if (DropSystem.Instance)
                DropSystem.Instance.RegisterDestructibleDrop(stats);
            //Debug.Log("Registered to drop system");
        }
        //rb = GetComponent<Rigidbody>();
    }
    #endregion
    #region FUNCTIONALITY
    public virtual void TakeDamage(DamageArgs args)
    {
        // DODGE -- potential to miss, no damage taken
        float dodge = Random.Range(0.0f, 100.0f); // float inclusive 0-100
        if (dodge <= stats.Dodge.Value)
            return;

        if (args.isCrit)
            CombatFeedbackManager.Instance.PlayCritParticles(transform.position);
        else
            CombatFeedbackManager.Instance.PlayHitParticles(transform.position);

        float recalc = args.amount * Mathf.Clamp01(1.0f - ((stats.Armour.Value * GameManager.Instance.ArmourConvert) / 100));
        // 100 damage incoming - 400 armour stat, conversion is 0.05
        // 100 * # between 0-1 -> 1 would be no resistance, 0 would full resistance
        // 100 * (1 - ((400 * 0.05) / 100))
        // 100 * (1 - (20 / 100))
        // 100 * (1 - 0.2)
        // 100 * (0.8)
        // 80 damage taken --> 20% armour reduction

        // How do we get the player's active ability
        // Does the player gain the mana reduction to damage if they have selected salamander's gift as a fusion


        // When taking damage, if the player has the elementalist's gift active
        // Then take a portion of the damage as mana instead of health
        // How will the hurtbox know to do that?

        // Simple - DamageFromMana stat has been implemented
        // Factor this into the health recalculation before applying the damage to the player
        // This is ONLY factored in, if the given stats can be a player stat. Therefore
        if (stats is PlayerStats ps)
        {
            // Take the recalculated value, and apply a portion of it to the mana
            // Get the percentage that is supposed to be reduced
            float percent = ps.DamageFromMana.Value / 100.0f;   // 25 / 100 = 0.25 --> incoming damage = 100, 100 * 0.25 = 25 to apply to mana, use the inverse for health
            // How to get the inversed? 1 - 0.25 = 0.75
            ps.Health.Value -= recalc * (1 - percent);
            ps.Mana.Value -= recalc * percent;
            Debug.Log("[HurtboxSource]: Took: " + (recalc * (1 - percent)) + " from health");
            Debug.Log("[HurtboxSource]: Took: " + (recalc * percent) + " from mana");
        }
        else
        {
            // Standard health deduction
            stats.Health.Value -= recalc;
            Debug.Log("[HurtboxSource]: Took: " + recalc);
        }

        if (applyKnockback)
        {
            HandleKnockback(args);
        }

        FireOnDamaged(stats.Health.Value);
        //Debug.Log("[DamageSystem]: " + gameObject.name + " took " + damage + " damage, value changed from (" + stats.Health.OldValue + " to " + stats.Health.Value + ")");
    }

    [SerializeField] float knockbackForce = 1.0f;
    [SerializeField] float knockbackTime = 0.5f;
    public virtual void HandleKnockback(DamageArgs args)
    {
        if (!applyKnockback || args.source == null) { return; }    // If you cannot knockback, or the source of the damage is null, return
        // Rigidbody rb = GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.AddForce(dir * knockbackForce * Time.deltaTime);
        // }  
        //applyKnockback = false;
        Vector3 dir = (transform.position - args.source.transform.position);    // my position - where was the damage from
        dir.y = 0.0f;
        if (knockback != null)
        {
            StopCoroutine(knockback);
        }
        knockback = StartCoroutine(ApplyKnockback(dir));
        //dir.y = 0;
        //transform.position += dir * knockbackForce; // * Time.deltaTime;
    }

    Coroutine knockback;

    IEnumerator ApplyKnockback(Vector3 dir)
    {
        float timer = 0.0f;
        Vector3 start = transform.position;
        Vector3 end = start + dir * knockbackForce;
        while (timer < knockbackTime)
        {
            float offset = timer / knockbackTime;
            transform.position = Vector3.Lerp(start, end, offset);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        //applyKnockback = true;
    }
    #endregion

    protected void OnMouseOver()
    {
        GameplayUIController.Instance.FireMouseHovered(new MouseHoverEventArgs(this.gameObject));
    }
    protected void OnMouseExit()
    {
        GameplayUIController.Instance.FireMouseExit(new MouseHoverEventArgs(null));
    }
}
