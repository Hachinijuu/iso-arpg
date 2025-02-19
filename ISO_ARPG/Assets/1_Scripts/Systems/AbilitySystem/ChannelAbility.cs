using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelAbility : Ability
{
    // Because it is derived from ability, it can include the unique attributes of the type

    //public bool Channelable {get {return canChannel;}}
    public float Interval { get { return interval; } }
    public bool ManaPerTick { get { return manaPerTick; } }
    //private bool canChannel = true; // Because it is a channel-able ablility, this is always true.
    [Header("Channel Settings")]
    [SerializeField] protected float interval = -1f;
    [SerializeField] bool manaPerTick = false;

    // Overrides abstract Fire method, but does not do anything
    // This acts as a base class to Channelable abilities (easier ability sorting)
    protected override void Fire(Ability ab, GameObject actor) {}

    // OnTick is virtual so derived classes DO NOT need to override the method.
    // This is handled externally, by the ability handler
    public virtual void OnTick()
    {

    }

}
