using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbility : Ability
{
    public float ActiveTime { get { return activeTime; } }
    public float IntervalTime { get { return interval; } }
    
    [Header("Passive Settings")]
    [SerializeField] protected float activeTime = -1f;      // the active time window of the skills activity
    [SerializeField] protected float interval = -1f;        // the interval time between when ticks occur

    float curTime;
    float intervalTime;

    // Overrides abstract Fire method, but does not do anything
    // This acts as a base class to passive abilities (easier ability sorting)

    // FIRE is when the passive ability has been activated
    protected override void Fire(Ability ab, GameObject actor) {}

    // Because update is not called on scriptable objects, THIS is called by the ability handler on any passive ability

    // OnTick is virtual so derived classes DO NOT need to override the method.
    // This is handled externally, by the ability handler
    public virtual void OnTick()
    { 
    
    }
}
