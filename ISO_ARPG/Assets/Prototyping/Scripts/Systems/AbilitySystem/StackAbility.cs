using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackAbility : Ability
{
    public int Stacks { get { return stacks; }  }
    [SerializeField] protected int stacks = 0;

    public int MaxStacks { get { return maxStacks; } }
    [SerializeField] protected int maxStacks = 0;

    [Header("Stack Effects")]
    [SerializeField] protected List<SubStat> StackStats;

    // Stack Stats defines WHICH stat type of stat will be incresed (movespeed,attackspeed,etc)
    // The value assigned to the stat is how much each stack will provide (i.e, 1 stack = 10 flat bonus, 2 stack = 20)
    protected override void Fire(Ability ab, GameObject actor)
    {
        throw new System.NotImplementedException();
    }


    // Calculation will take the stackedStat's values, modify them, and return and UPDATED stat
    // The updated stat should be plugged into whichever relevant stats are updated.
    // i.e playerStat.STR.Value += CalculateStat(strengthStack);
    // This does not account for the original player's stat values, so it is an additive number (unless * operator for multipled increase)
    protected virtual SubStat CalculateStat(SubStat stackedStat)
    {
        SubStat updatedStat = new SubStat(stackedStat); // Get a copy of the base value
        updatedStat.Value *= stacks;                    // Get the new value based on the number of stacks
        return updatedStat;                             // Return the increased value
    }

    protected virtual SubStat GetSubStatFromType(SubStatTypes subStatType) 
    {
        foreach (SubStat subStat in StackStats)
        { 
            if (subStat.type == subStatType) return subStat;
        }
        return null;
    }

    public void ResetStacks()
    {
        stacks = 0;
    }
}
