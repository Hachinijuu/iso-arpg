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

    public float maxRollDifficultyStep = 0.2f;
    [Tooltip("Max * minimumPercentage (100 * 0.6) = 60 min value")]
    public float minimumPercentage = 0.6f;

    public GameObject upgradeUI;

    //public void Awake()
    //{
    //    mainStatRolls = new List<MainStatRoll>();
    //    trackedStatRolls = new List<TrackedStatRoll>();
    //    substatRolls = new List<SubStatRoll>();
    //}

    public RuneData CreateMainStatRune(ItemRarity rank, MainStatTypes type)
    {
        RuneData rune = new RuneData();
        MainStat stat = new MainStat(type, -1);     // Value less 0 than can be rolled for
        rune.mainStat = new MainStat[1] { stat };   // Add the stat to the array of stats
        rune = RollRune(rune, rank);

        // Add a random type to it
        rune.type = ItemTypes.RUNE;
        rune.runeType = (RuneType)Random.Range(1, 4);
        return rune;
    }

    public RuneData CreateSubStatRune(ItemRarity rank, SubStatTypes type)
    {
        RuneData rune = new RuneData();
        SubStat stat = new SubStat(type, -1);     // Value less 0 than can be rolled for
        rune.subStat = new SubStat[1] { stat };   // Add the stat to the array of stats

        rune.type = ItemTypes.RUNE;
        rune.runeType = (RuneType)Random.Range(1, 4);
        rune = RollRune(rune, rank);
        return rune;
    }

    public RuneData RollRune(RuneData template)
    {
        RuneData rune = template;
        float rarityRoll = Random.Range(0, 100f);
        // Roll for the difficulty

        DifficultySetting difficultyMod = GameManager.Instance.currDifficulty;
        foreach (RarityChances chance in difficultyMod.dropModifiers)
        {
            // If I get a 30 roll on dropValue
            // And the chance for a Relic tier rune is 5%, I get that rune
            // Since the loop will check for the chances based on sequential order, (common -> relic)
            // The rarity will be update based on the lowest drop chance value

            if (chance.dropChance < rarityRoll)
            {
                rune.rarity = chance.rarity;
            }
        }
        rune = RollRuneStats(rune);
        return rune; // Roll for the stats
    }

    // This is an overload of rune rolling, with the item rarity. It is expected for upgrades, since the rarity is set on the increased rune
    // I.e, for the upgrade, the rarity is increased by 1 in the upgrade screen
    public RuneData RollRune(RuneData template, ItemRarity rarity)
    {
        RuneData rune = template;
        rune.rarity = rarity;
        rune = RollRuneStats(rune);
        return rune;
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
                        switch (PlayerManager.Instance.currentClass)
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

                        // rarity tier 3 --> 3 * maxRollDifficultyStep = 0.6 + 1 = 1.6, base and increase * default max
                        float maxRoll = rollRange.maxValue * (1 + ((int)rune.rarity * maxRollDifficultyStep));
                        float minRoll = maxRoll * minimumPercentage;                      // 60% of max roll, min can be 40% lower

                        // Set the max value here
                        mainStat.MaxValue = maxRoll;
                        mainStat.Value = (int)Random.Range(minRoll, maxRoll);     // Roll for the stat based on the number
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
                        float maxRoll = rollRange.maxValue * (1 + ((int)rune.rarity * maxRollDifficultyStep));
                        float minRoll = maxRoll * minimumPercentage;
                        trackedStat.Value = (int)Random.Range(minRoll, maxRoll);
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
                        if (subStat.Value > 0)  // IF THE VALUE ALREADY EXISTS IN THE RUNE,  DO NOT RANDOMIZE IT
                            continue;
                        float maxRoll = rollRange.maxValue * (1 + ((int)rune.rarity * maxRollDifficultyStep));
                        float minRoll = maxRoll * minimumPercentage;

                        subStat.Value = (int)Random.Range(minRoll, maxRoll);
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
