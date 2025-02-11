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

        // ARMOUR -- damage reduction
        // 50 incoming

        // armour value
        // 400 * 0.005 = 20
        // conversion is 

        float recalc = (damage * (1 - (stats.Armour.Value * GameManager.Instance.ArmourConvert)));
        stats.Health.Value -= recalc;
        FireOnDamaged(stats.Health.Value);
        //Debug.Log("[DamageSystem]: " + gameObject.name + " took " + damage + " damage, value changed from (" + stats.Health.OldValue + " to " + stats.Health.Value + ")");
    }
    #endregion
}
