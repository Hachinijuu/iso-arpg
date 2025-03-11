using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "sykcorSystems/Entities/Data", order = 1)]
public class EntityData : ScriptableObject
{
    #region Class Description
    [Header("Descriptions")]
    public string entityName;
    public string description;
    public Sprite icon;
    #endregion

    // Add the shared stats that all entities have here.
    [Header("Stats")]
    public TrackedStat Health;
    
    [Header("Offensive Stats")]
    public SubStat Damage;
    public SubStat AttackSpeed;

    [Header("Defensive Stats")]
    public SubStat Armour;
    public SubStat Dodge;

    [Header("Utility Stats")]
    public SubStat MoveSpeed;
}
