using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rune types must match the rune slot for it to be set properly, if it is not - it is an invalid slot
public enum RuneType
{
    NONE,
    OFFENSE,
    DEFENSE,
    UTILITY
}

[CreateAssetMenu(fileName = "Rune", menuName = "sykcorSystems/Items/Runes", order = 1)]
public class RuneData : ItemData
{
    public RuneType runeType;
    public MainStat mainStat;
    public SubStat subStat;
    public virtual void ApplyStats(ref PlayerStats stats)
    {
        foreach (Stat stat in stats.statList)
        {
            if (stat is MainStat ms)
            {
                if (ms.type == mainStat.type)
                {
                    stat.Value += mainStat.Value;
                }
            }
            if (stat is SubStat sb)
            {
                if (sb.type == subStat.type)
                {
                    stat.Value += subStat.Value;
                }
            }
        }
    }

    public virtual void RemoveStats(ref PlayerStats stats)
    {
        foreach (Stat stat in stats.statList)
        {
            if (stat is MainStat ms)
            {
                if (ms.type == mainStat.type)
                {
                    stat.Value -= mainStat.Value;
                }
            }
            if (stat is SubStat sb)
            {
                if (sb.type == subStat.type)
                {
                    stat.Value -= subStat.Value;
                }
            }
        }
    }
}
