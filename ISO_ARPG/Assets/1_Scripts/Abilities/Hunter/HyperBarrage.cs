using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HyperBarrage", menuName = "sykcorSystems/Abilities/Hunter/HyperBarrage", order = 4)]
public class HyperBarrage : Ability
{
    #region VARIABLES
    Animator anim;
    AudioSource source;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] int baseProjectiles;   // This is the base amount of projectiles that exists with this ability
    int numProjectiles; // This is the actual amount of projectiles shot with this ability
    #endregion
    protected override void Fire(Ability ab, GameObject actor)
    {
        
    }
}
