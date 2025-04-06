using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RuneContainer", menuName = "sykcorSystems/Drops/RuneTable", order = 2)]
public class RuneTable : ScriptableObject
{
    public List<DropTableModifier> runes;
}
