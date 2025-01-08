using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This stat class is to organize data in a more effective manner
// With customized types, comparing and referencing stat combinations should be easier
// i.e Gear with x stats gives bonus

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
    CRIT_CHANCE     // chance to crit
}
#endregion

#region STAT CLASSES

#region Framework
// Base class for stats, adheres to no stat type
public class Stat
{ 
    // Data
    public virtual float Value { get { return value; } set { SetValue(value); } }
    protected float value;

    // Constructors
    public Stat() { value = -1; }
    public Stat(float value) { this.value = value; }

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
}

// Stat that has an upper limit that it will not exceed.
public class ClampedStat : Stat
{
    // Public accessors
    public override float Value { get { return value; } set { SetClampedValue(value, StatLimits.STAT_MAX); } }

    // Constructors
    public ClampedStat() : base() { }
    public ClampedStat(float value) : base(value) { }

    // Functionality
    public virtual void SetClampedValue(float value, float maxValue)
    {
        if (this.value + (value - this.value) <= maxValue)   // if the current value + the difference between the new value is less than the max
        {
            this.value = value;                                         // use the requested value
        }
        else if (this.value + (value - this.value) <= 0)                // if the current value + the difference between the new value is less than zero
        {
            this.value = 0;                                             // use the zero (NOTE: if we want negative stats, this check can be removed)
        }
        else                                                            // otherwise, the stat exceeds the max value
        { 
            this.value = StatLimits.STAT_MAX;                           // clamp the stat to the max provided
        }
        FireChanged(value); // tell the listeners the new value
    }
}
#endregion

#region Gameplay

// Main stats keep track of which type they are bound to
public class MainStat : ClampedStat
{
    // Data
    public MainStatTypes type;

    // Constructors
    public MainStat() : base() { type = MainStatTypes.NONE; }
    public MainStat(MainStatTypes type, float value) : base(value) { this.type = type; }
}

public class SubStat : ClampedStat
{
    // Data
    public SubStatTypes type;

    // Constructors
    public SubStat() : base() { type = SubStatTypes.NONE; }
    public SubStat(SubStatTypes type, float value) : base(value) { this.type = type; }
}

public class TrackedStat : ClampedStat
{
    // Data
    public TrackedStatTypes type;
    protected float oldValue;
    protected float maxValue;

    // Public Data Accessors
    public float OldValue { get { return oldValue; } }

    public float MaxValue { get { return maxValue; }
        set
        {
            if (maxValue + value <= StatLimits.TRACKED_STAT_MAX)
                maxValue = value;
            else if (maxValue + value <= 0)
                maxValue = 0;
            else
                maxValue = StatLimits.TRACKED_STAT_MAX;
        }
    }

    // Constructors
    public TrackedStat() : base() { }
    public TrackedStat(TrackedStatTypes type, float value) : base(value) { type = TrackedStatTypes.NONE; }
    public TrackedStat(TrackedStatTypes type, float value, float maxValue)
    { 
        type = TrackedStatTypes.NONE;
        MaxValue = maxValue;                    // This will set the classes' maxValue to the value passed in, and do bounds checking
        SetClampedValue(value, this.maxValue);  // Clamp the passed in value to the max value
    }

    // Functionality
    public override void SetClampedValue(float value, float maxValue)
    {
        oldValue = this.value;                  // Store the old value
        base.SetClampedValue(value, maxValue);  // Update the value to the new one
        FireChanged(value);                     // Tell the listeners the new value
    }
}

#endregion

#endregion