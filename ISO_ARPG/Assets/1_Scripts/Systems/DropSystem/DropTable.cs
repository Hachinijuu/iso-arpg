using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "sykcorSystems/Drops/DropTable", order = 1)]
public class DropTable : ScriptableObject
{
    public EntityID dropId;
    public List<ItemData> items;
}
