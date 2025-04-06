using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "sykcorSystems/Drops/DropTable", order = 1)]
public class DropTable : ScriptableObject
{
    public EntityID dropId;
    public List<DropTableModifier> items;
    public RuneTable runesTable;
}

[System.Serializable]
public class DropTableModifier
{
    public ItemData item;
    
    [Range(0, 100)]
    public int dropModifier = 0;
}
