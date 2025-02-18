using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    #region VARIABLES
    // THIS IS NOT FINAL IMPLEMENTATION
    // IT WILL BE FLESHED ONCE STAT LOADING AND CHARACTER INFORMATION IS ORGANIZED

    // Get a reference to the health on the specified object
    [SerializeField] protected EntityStats stats;
    #endregion
    //TrackedStat health;

    #region EVENTS
    // Fire event whenever this hurtbox takes damage
    public delegate void OnDamaged(float value);
    public OnDamaged onDamaged;

    private void FireOnDamaged(float value) { if (onDamaged != null) { onDamaged(value); } }
    #endregion
    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("[DamageSystem]: Missing stat reference");
        }
    }
    #endregion
    #region FUNCTIONALITY
    public virtual void TakeDamage(float damage)
    {
        // DODGE -- potential to miss, no damage taken
        float dodge = Random.Range(0.0f, 100.0f); // float inclusive 0-100
        if (dodge <= stats.Dodge.Value)
            return;

        float recalc = damage * Mathf.Clamp01(1.0f - ((stats.Armour.Value * GameManager.Instance.ArmourConvert) / 100));
        // 100 damage incoming - 400 armour stat, conversion is 0.05
        // 100 * # between 0-1 -> 1 would be no resistance, 0 would full resistance
        // 100 * (1 - ((400 * 0.05) / 100))
        // 100 * (1 - (20 / 100))
        // 100 * (1 - 0.2)
        // 100 * (0.8)
        // 80 damage taken --> 20% armour reduction

        stats.Health.Value -= recalc;
        FireOnDamaged(stats.Health.Value);
        //Debug.Log("[DamageSystem]: " + gameObject.name + " took " + damage + " damage, value changed from (" + stats.Health.OldValue + " to " + stats.Health.Value + ")");
    }
    #endregion
}
