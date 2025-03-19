using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "sykcorSystems/Drops/Potion", order = 4)]
public class PotionData : ItemData
{
    public PotionTypes potionType;
    public float value;
}
