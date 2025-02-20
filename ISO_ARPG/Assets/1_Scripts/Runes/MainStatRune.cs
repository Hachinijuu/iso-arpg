using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Main Stat Rune", menuName = "sykcorSystems/Items/Runes", order = 2)]
public class MainStatRune : RuneData
{
    public MainStat stat;
    public RuneMathType applyType;
    public override void ApplyStats(ref PlayerStats stats)
    {
        foreach (Stat listStat in stats.statList)
        {
            // Find what matches this main stat
            MainStat compare = listStat as MainStat;
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
