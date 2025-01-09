using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbility : Ability
{
    public float ActiveTime { get { return activeTime; } }
    [SerializeField] protected float activeTime = -1f;      // the active time window of the skills activity

    // Overrides abstract Fire method, but does not do anything
    // This acts as a base class to passive abilities (easier ability sorting)
    protected override void Fire(Ability ab, GameObject actor) {}
}
