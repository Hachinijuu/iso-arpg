using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "sykcorSystems/Drops/Resource", order = 5)]
public class ResourceData : ItemData
{
    public ResourceTypes resourceType;
    public int amount;
}
