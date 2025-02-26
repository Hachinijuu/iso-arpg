using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rune", menuName = "sykcorSystems/Items/Runes", order = 1)]
public class RuneData : ItemData
{
    public MainStat mainStat;
    public SubStat subStat;
    public virtual void ApplyStats(ref PlayerStats stats)
    {
        foreach (Stat listStat in stats.statList)
        {
            if (listStat is MainStat ms)
            {
                if (ms.type == mainStat.type)
                {
                    ms.Value += mainStat.Value;
                }
            }
            if (listStat is SubStat sb)
            {
                if (sb.type == subStat.type)
                {
                    sb.Value += subStat.Value;
                }
            }
        }
    }
}
