using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Rune types must match the rune slot for it to be set properly, if it is not - it is an invalid slot
public enum GearSlotType
{
    NONE,
    HEAD,
    CHEST,
    LEGS,
    BOOTS,
    NECKLACE,
    RING,
    BELT,
    WEAPON
}

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
    public Sprite runeGlyph;
    // How to do typematching for stats...
    // If no stat exists, don't display the value visually in tooltips
    // Only display if the statType is not none

    public RuneType runeType;
    public MainStat[] mainStat;
    public TrackedStat[] trackedStat;
    public SubStat[] subStat;

    // public void RuneData(RuneData copy)
    // {
    //     runeGlyph = copy.runeGlyph;
    //     runeType = copy.runeType;
    //     mainStat = copy.mainStat;
    //     trackedStat = copy.trackedStat;
    //     subStat = copy.subStat;
    // }

    public int destroyAmount;   // The amount of dust returned on rune destruction
    public virtual void ApplyStats(ref PlayerStats stats)
    {
        foreach (Stat listStat in stats.statList)
        {
            if (listStat is MainStat && mainStat != null && mainStat.Length > 0)
            {
                MainStat ms = listStat as MainStat;
                foreach (MainStat stat in mainStat)
                {
                    if (ms.type == stat.type)
                    {
                        ms.Value += stat.Value;
                        Debug.Log("[Runes]: Added " + stat.Value + " " + stat.type);
                    }
                }
            }
            else if (listStat is TrackedStat && trackedStat != null && trackedStat.Length > 0)
            {
                TrackedStat ts = listStat as TrackedStat;
                foreach (TrackedStat stat in trackedStat)
                {
                    if (ts.type == stat.type)
                    {
                        // NOTE, only if it is a tracked stat, can the value exceed the maximum, this is because the maximum values are GAMEPLAY max's
                        ts.MaxValue += stat.MaxValue;
                        //ts.Value += stat.Value;
                        Debug.Log("[Runes]: Added " + stat.Value + " " + stat.type);
                    }
                }
            }
            else if (listStat is SubStat && subStat != null && subStat.Length > 0)
            {
                SubStat ss = listStat as SubStat;
                foreach (SubStat stat in subStat)
                {
                    if (ss.type == stat.type)
                    {
                        ss.Value += stat.Value;
                        Debug.Log("[Runes]: Added " + stat.Value + " " + stat.type);
                    }
                }
            }
        }


        // if (mainStat != null && mainStat.Length > 0)
        // {
        //     foreach (Stat listStat in stats.statList)
        //     {
        //         if (listStat is MainStat)
        //         {
        //             //MainStat ms = listStat as MainStat;
        //             foreach (MainStat stat in mainStat)
        //             {
        //                 if (listStat.type == stat.type)
        //                 {
        //                     listStat.Value += stat.Value;
        //                 }
        //             }
        //         }
        //     }
        // }
        // else if (trackedStat != null && trackedStat.Length > 0)
        // {
        //     foreach (Stat listStat in stats.statList)
        //     {
        //         if (listStat is TrackedStat)
        //         {
        //             //listStat as TrackedStat;
        //             foreach (TrackedStat stat in trackedStat)
        //             {
        //                 if (listStat.type == stat.type)
        //                 {
        //                     listStat.Value += stat.Value;
        //                 }
        //             }
        //         }
        //     }
        // }
        // else if (subStat != null && subStat.Length > 0)
        // {
        //     foreach (SubStat listStat in stats.statList)
        //     {
        //         foreach (SubStat stat in subStat)
        //         {
        //             if (listStat.type == stat.type)
        //             {
        //                 listStat.Value += stat.Value;
        //             }
        //         }
        //     }
        // }

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
        foreach (Stat listStat in stats.statList)
        {
            if (listStat is MainStat && mainStat != null && mainStat.Length > 0)
            {
                MainStat ms = listStat as MainStat;
                foreach (MainStat stat in mainStat)
                {
                    if (ms.type == stat.type)
                    {
                        ms.Value -= stat.Value;
                        Debug.Log("[Runes]: Removed " + stat.Value + " " + stat.type);
                    }
                }
            }
            else if (listStat is TrackedStat && trackedStat != null && trackedStat.Length > 0)
            {
                TrackedStat ts = listStat as TrackedStat;
                foreach (TrackedStat stat in trackedStat)
                {
                    if (ts.type == stat.type)
                    {
                        //ts.Value -= stat.Value;
                        ts.MaxValue -= stat.MaxValue;
                        Debug.Log("[Runes]: Removed " + stat.Value + " " + stat.type);
                    }
                }
            }
            else if (listStat is SubStat && subStat != null && subStat.Length > 0)
            {
                SubStat ss = listStat as SubStat;
                foreach (SubStat stat in subStat)
                {
                    if (ss.type == stat.type)
                    {
                        ss.Value -= stat.Value;
                        Debug.Log("[Runes]: Removed " + stat.Value + " " + stat.type);
                    }
                }
            }
        }
    }
}
