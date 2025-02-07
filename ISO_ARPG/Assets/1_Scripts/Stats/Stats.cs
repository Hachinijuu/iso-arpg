// This stat class is to organize data in a more effective manner
// With customized types, comparing and referencing stat combinations should be easier
// i.e Gear with x stats gives bonus
using System;
using UnityEngine;

#region STAT LIMITS
// This class contains the upper limits of stats


public static class StatLimits
{
    public const int MOVE_MAX = 10;
    public const int STAT_MAX = 100000;
    public const int TRACKED_STAT_MAX = 10000;

    // because of the nature of the base class and types, specific limits can be included if bound to checking associated stat in derived classes
    // generally these limits are imposed to stop complete gamebreaking scenarios by keeping player stats within a certain threshold
}
#endregion

#region STAT TYPES
// Main Stat types are stats that every class will have by default, they are STANDALONE stats that INFLUENCE other stats
public enum MainStatTypes
{
    NONE,
    STRENGTH,       // modifier to berserker damage, health and armour
    DEXTERITY,      // modifier to hunter damage, speed and dodge
    INTELLIGENCE,   // modifier to elementalist damage, mana and health
    HASTE,          // modifies attack speed and movement speed
}

// Tracked Stats are stats that will change in value during gameplay, they keep track of current and maximum values
public enum TrackedStatTypes
{
    NONE,
    HEALTH,
    MANA,
    ID_BAR
}

// Substats are stats that have a direct purpose, they may be infuenced by mains states
public enum SubStatTypes
{
    NONE,
    DAMAGE,         // damage
    MOVE_SPEED,     // movement speed
    ATTACK_SPEED,   // attack speed
    ARMOUR,         // damage reduction
    DODGE,          // chance to dodge
    CRIT_CHANCE,    // chance to crit
    CRIT_DAMAGE,    // damage of critical strikes
}
#endregion

#region STAT CLASSES

#region Framework
// Base class for stats, adheres to no stat type

[System.Serializable]
public class Stat
{
    // Data
    public virtual float Value { get { return value; } set { SetValue(value); } }

    [SerializeField] protected float value;

    // Constructors
    public Stat() { value = -1; }
    public Stat(float value) { this.value = value; }
    public Stat(Stat copy)
    {
        Copy(copy);
    }

    // Event Callbacks
    public delegate void ValueChanged(float value);
    public event ValueChanged Changed;
    protected virtual void FireChanged(float value) { if (Changed != null) Changed(value); }

    // Functionality
    public virtual void SetValue(float value)
    {
        this.value = value;
        FireChanged(value); // Tell the listeners the new value
    }

    public virtual void Copy(Stat other)
    { 
        this.value = other.value;
    }
}

// Stat that has an upper limit that it will not exceed.
[System.Serializable]
public class ClampedStat : Stat
{
    [SerializeField] protected float maxValue;
    // Public accessors
    //public override float Value { get { return value; } set { SetClampedValue(value); } }
    public float MaxValue
    {
        get { return maxValue; }
        set
        {
            if (value <= StatLimits.TRACKED_STAT_MAX)
                maxValue = value;
            else if (value <= 0)
                maxValue = 0;
            else
                maxValue = StatLimits.TRACKED_STAT_MAX;
        }
    }

    // Constructors
    public ClampedStat() : base() { }
    public ClampedStat(float value) : base(value) { this.maxValue = StatLimits.STAT_MAX; }
    public ClampedStat(float value, float maxValue) : base(value) { this.maxValue = maxValue; }
    public ClampedStat(ClampedStat copy)
    { 
        Copy(copy);
    }

    // Functionality
    public override void SetValue(float value)
    {
        if (value > maxValue)
        {
            this.value = maxValue;
        }
        else if (value < 0)
        {
            this.value = 0;
        }
        else
        {
            this.value = value;
        }
        FireChanged(this.value); // tell the listeners the new value
        
        //this.value = value;
        //if (this.value > maxValue)
        //    this.value = maxValue;
        //if (this.value + (value - this.value) < maxValue)   // if the current value + the difference between the new value is less than the max
        //{
        //    if (value - this.value < 0)
        //        return;
        //    else
        //        this.value = value;     // use the requested value
        //}
        //else if (this.value + (value - this.value) < 0)                // if the current value + the difference between the new value is less than zero
        //{
        //    this.value = 0;                                             // use the zero (NOTE: if we want negative stats, this check can be removed)
        //}
        //else                                                            // otherwise, the stat exceeds the max value
        //{
        //    this.value = maxValue;                           // clamp the stat to the max provided
        //}
    }
}
#endregion

#region Gameplay

// Main stats keep track of which type they are bound to
[System.Serializable]
public class MainStat : ClampedStat
{
    // Data
    public MainStatTypes type;

    // Constructors
    public MainStat() : base() { type = MainStatTypes.NONE; }
    public MainStat(MainStatTypes type, float value) : base() { this.type = type; }

    public virtual void Copy(MainStat other)
    {
        this.value = other.value;
        this.type = other.type;
    }

    public MainStat(MainStat copy)
    {
        Copy(copy);
    }
}

[System.Serializable]
public class SubStat : ClampedStat
{
    // Data
    public SubStatTypes type;

    // Constructors
    public SubStat() : base() { type = SubStatTypes.NONE; }
    public SubStat(SubStatTypes type, float value) : base(value) { this.type = type; }

    public SubStat(SubStat copy)
    { 
        Copy(copy);
    }

    public virtual void Copy(SubStat other)
    {
        this.value = other.value;
        this.type = other.type;
    }
}

[System.Serializable]
public class TrackedStat : ClampedStat
{
    // Data
    public TrackedStatTypes type;
    protected float oldValue;

    // Public Data Accessors
    //public override float Value { get { return base.value; } set { SetClampedValue(value); } }
    public float OldValue { get { return oldValue; } }

    // Constructors
    public TrackedStat() : base() { }
    public TrackedStat(TrackedStatTypes type, float value) : base(value) { type = TrackedStatTypes.NONE; }
    public TrackedStat(TrackedStatTypes type, float value, float maxValue)
    {
        type = TrackedStatTypes.NONE;
        MaxValue = maxValue;                    // This will set the classes' maxValue to the value passed in, and do bounds checking
        SetValue(value);                 // Clamp the passed in value to the max value
    }
    public TrackedStat(TrackedStat copy)
    { 
        Copy(copy);
    }


    // Functionality
    public override void SetValue(float value)
    {
        oldValue = this.value;                  // Store the old value
        base.SetValue(value);  // Update the value to the new one
        //FireChanged(this.value);                     // Tell the listeners the new value
    }

    public virtual void Copy(TrackedStat other)
    {
        this.value = other.value;
        this.oldValue = other.oldValue;
        this.maxValue = other.maxValue;
        this.type = other.type;
    }
}

#endregion

#endregion