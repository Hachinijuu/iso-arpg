using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    // Difference between enemyStats and entityStats
    // Is that enemies contain a list of possible states they can use

    // Since the asset that holds the states only exists once, all agents simply get a reference to the states
    // And how they should behave given their current states
    public StateContainer agentStates;
}
