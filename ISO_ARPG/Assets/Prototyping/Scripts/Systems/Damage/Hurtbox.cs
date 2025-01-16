using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    // THIS IS NOT FINAL IMPLEMENTATION
    // IT WILL BE FLESHED ONCE STAT LOADING AND CHARACTER INFORMATION IS ORGANIZED

    // Get a reference to the health on the specified object
    [SerializeField] EntityStats stats;
    
    //TrackedStat health;

    // Start is called before the first frame update
    void Start()
    {
        if (stats == null)
        {
            Debug.LogError("[DamageSystem]: Missing stat reference");
        }
    }

    public void TakeDamage(int damage)
    {
        stats.Health.Value -= damage;
        Debug.Log("[DamageSystem]: " + gameObject.name + " took " + damage + " damage, value changed from (" + stats.Health.OldValue + " to " + stats.Health.Value + ")");
    }
}
