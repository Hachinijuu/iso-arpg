using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MainStatRoll
{
    public MainStatTypes type;
    public float maxValue;
}
[System.Serializable]
public struct TrackedStatRoll
{
    public TrackedStatTypes type;
    public float maxValue;
}
[System.Serializable]
public struct SubStatRoll
{
    public SubStatTypes type;
    public float maxValue;
}


public class RuneSystem : MonoBehaviour
{
    private static RuneSystem instance;
    public static RuneSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RuneSystem>();
            if (!instance)
                Debug.LogWarning("[RuneSystem]: No Debug Menu found");
            return instance;
        }
    }

    public List<MainStatRoll> mainStatRolls;
    public List<TrackedStatRoll> trackedStatRolls;
    public List<SubStatRoll> substatRolls;

    public float RollUpperStep = 0.1f;

    public void Awake()
    {
        mainStatRolls = new List<MainStatRoll>();
        trackedStatRolls = new List<TrackedStatRoll>();
        substatRolls = new List<SubStatRoll>();
    }

    public RuneData RollRuneStats(RuneData template)
    {
        RuneData rune = template;   
        // Create a copy of the rune that should be rolled for
        // Roll the statistics for that rune


        // How does a rune roll work

        // Rune contains a number of stats
        // Match the runeStat to it's roll type

        // before doing stat rolls, roll for a rune's rarity
        // this roll is based on the difficulty

        if (rune.mainStat != null && rune.mainStat.Length > 0)
        {
            foreach (MainStat mainStat in rune.mainStat)
            {
                // Need to get the matching roll
                foreach (MainStatRoll rollRange in mainStatRolls)
                {
                    if (mainStat.type == MainStatTypes.NONE)
                    {
                        // Match to the player class
                        switch(PlayerManager.Instance.currentClass)
                        {
                            case GoX_Class.BERSERKER:
                                mainStat.type = MainStatTypes.STRENGTH;
                            break;
                            case GoX_Class.HUNTER:
                                mainStat.type = MainStatTypes.DEXTERITY;
                            break;
                            case GoX_Class.ELEMENTALIST:
                                mainStat.type = MainStatTypes.INTELLIGENCE;
                            break;
                        }
                    }
                    if (mainStat.type == rollRange.type)  // If the stat to change matches the rollRange -- if the rune has a strength stat, look for the strength roll
                    {
                        if (mainStat.Value > 0) // If the value is defined, keep that value instead continue
                            continue;
                        float minRoll = rollRange.maxValue * 0.6f;                      // 60% of max roll, min can be 40% lower
                        mainStat.Value = Random.Range(minRoll, rollRange.maxValue * (1 + ((int)rune.rarity * RollUpperStep)));     // Roll for the stat based on the number
                    }
                }
            }
        }
        if (rune.trackedStat != null && rune.trackedStat.Length > 0)
        {
            foreach (TrackedStat trackedStat in rune.trackedStat)
            {
                foreach (TrackedStatRoll rollRange in trackedStatRolls)
                {
                    if (trackedStat.type == rollRange.type)
                    {
                        if (trackedStat.Value > 0)
                            continue;
                        float minRoll = rollRange.maxValue * 0.6f;
                        trackedStat.Value = Random.Range(minRoll, rollRange.maxValue * (1 + ((int)rune.rarity * RollUpperStep)));
                    }
                }
            }
        }
        if (rune.subStat != null && rune.subStat.Length > 0)
        {
            foreach (SubStat subStat in rune.subStat)
            {
                foreach (SubStatRoll rollRange in substatRolls)
                {
                    if (subStat.type == rollRange.type)
                    {
                        if (subStat.Value > 0)
                            continue;
                        float minRoll = rollRange.maxValue * 0.6f;
                        subStat.Value = Random.Range(minRoll, rollRange.maxValue * (1 + ((int)rune.rarity * RollUpperStep)));
                    }
                }
            }
        }

        // which stats to roll for
        

        // roll the ranges for the given stats.. 
        // this will probably be hardcoded to match the types of the stats that exist and should be added

        // i.e projectile roll can range from 0-2 
        // with a projectile roll, the damage needs to be deducted accordingly
        // how will my rune know i need to deduct the amount based on the roll
        
        // the preset rune amount



        return rune;
    }

    // This class will handle rune upgrading as well, for sake of ease

    

    // Rune Rolling Based on Rarities
    // common

    // uncommon
    
    // rare
    
    // epic
    
    // relic
}
