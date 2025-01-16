using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    // All entites have a health stat.
    public TrackedStat Health { get { return health; } }

    // Made serializefield for debug purposes
    // Final implementation, stats are loaded externally
    [SerializeField] protected TrackedStat health;
}
