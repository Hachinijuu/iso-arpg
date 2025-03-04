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

[CreateAssetMenu(fileName = "Rune", menuName = "sykcorSystems/Drops/Rune", order = 3)]
public class RuneData : ItemData
{
    // How to do typematching for stats...
    // If no stat exists, don't display the value visually in tooltips
    // Only display if the statType is not none

    public RuneType runeType;
    public MainStat[] mainStat;
    public TrackedStat[] trackedStat;
    public SubStat[] subStat;
    public virtual void ApplyStats(ref PlayerStats stats)
    {
        if (mainStat != null && mainStat.Length > 0)
        {
            foreach (MainStat listStat in stats.statList)
            {
                foreach (MainStat stat in mainStat)
                {
                    if (listStat.type == stat.type)
                    {
                        listStat.Value += stat.Value;
                    }
                }
            }
        }
        else if (trackedStat != null && trackedStat.Length > 0)
        {
            foreach (TrackedStat listStat in stats.statList)
            {
                foreach (TrackedStat stat in trackedStat)
                {
                    if (listStat.type == stat.type)
                    {
                        listStat.Value += stat.Value;
                    }
                }
            }
        }
        else if (subStat != null && subStat.Length > 0)
        {
            foreach (SubStat listStat in stats.statList)
            {
                foreach (SubStat stat in subStat)
                {
                    if (listStat.type == stat.type)
                    {
                        listStat.Value += stat.Value;
                    }
                }
            }
        }

        //foreach (Stat stat in stats.statList)
        //{
        //    if (stat is MainStat ms)
        //    {
        //        if (ms.type == mainStat.type)       // If the type is none, this function should not get hit
        //        {
        //            stat.Value += mainStat.Value;
        //        }
        //    }
        //    if (stat is TrackedStat ts)
        //    {
        //        if (ts.type == trackedStat.type)
        //        {
        //            stat.Value += trackedStat.Value;
        //        }
        //    }
        //    if (stat is SubStat sb)
        //    {
        //        if (sb.type == subStat.type)
        //        {
        //            stat.Value += subStat.Value;
        //        }
        //    }
        //}
    }
    public virtual void RemoveStats(ref PlayerStats stats)
    {
        if (mainStat != null && mainStat.Length > 0)
        {
            foreach (MainStat listStat in stats.statList)
            {
                foreach (MainStat stat in mainStat)
                {
                    if (listStat.type == stat.type)
                    {
                        listStat.Value -= stat.Value;
                    }
                }
            }
        }
        else if (trackedStat != null && trackedStat.Length > 0)
        {
            foreach (TrackedStat listStat in stats.statList)
            {
                foreach (TrackedStat stat in trackedStat)
                {
                    if (listStat.type == stat.type)
                    {
                        listStat.Value -= stat.Value;
                    }
                }
            }
        }
        else if (subStat != null && subStat.Length > 0)
        {
            foreach (SubStat listStat in stats.statList)
            {
                foreach (SubStat stat in subStat)
                {
                    if (listStat.type == stat.type)
                    {
                        listStat.Value -= stat.Value;
                    }
                }
            }
        }
    }
}
