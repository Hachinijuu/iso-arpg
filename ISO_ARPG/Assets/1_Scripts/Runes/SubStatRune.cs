using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sub Stat Rune", menuName = "sykcorSystems/Items/Runes", order = 3)]
public class SubStatRune : RuneData
{
    public SubStat stat;
    public RuneMathType applyType;
    public override void ApplyStats(ref PlayerStats stats)
    {
        foreach (Stat listStat in stats.statList)
        {
            // Find what matches this main stat
            SubStat compare = listStat as SubStat;
            if (compare != null)
            {
                if (stat.type == compare.type)
                {
                    switch(applyType)
                    {
                        case RuneMathType.ADD:
                            listStat.Value += stat.Value;
                        break;
                        case RuneMathType.MULTIPLY:
                            listStat.Value *= stat.Value;
                        break;
                    }
                }
            }
        }
    }
}
