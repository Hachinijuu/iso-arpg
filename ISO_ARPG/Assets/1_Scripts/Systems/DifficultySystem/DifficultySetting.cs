using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "sykcorSystems/Difficulty", order = 1)]
public class DifficultySetting : ScriptableObject
{
    public float healthMultiplier = 1.0f;
    public float damageMultiplier = 1.0f;
    public int minEnemies = 30;
    public int maxEnemies = 50;
    public RarityChances[] dropModifiers;

    // TODO: Apply these changes to AI manager and enemies
    // Enemy damage will be relatively simple, their hitboxes have a static damage amount
    // When spawning the enemies, set the value of their hitboxes to be the flat damage amount * difficulty multiplier -- 50 base damage * 2 = 100 damage they will deal
    // Same for health, when initializing the enemy stats take their health value and multiply it
}

[System.Serializable]
public class RarityChances
{
    public ItemRarity rarity;
    public float dropChance;
}
