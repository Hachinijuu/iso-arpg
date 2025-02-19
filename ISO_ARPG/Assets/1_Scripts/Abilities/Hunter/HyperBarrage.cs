using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HyperBarrage", menuName = "sykcorSystems/Abilities/Hunter/HyperBarrage", order = 4)]
public class HyperBarrage : ChannelAbility
{
    // this is a fake passive ability, use it for the ticking functionality
    #region VARIABLES
    [SerializeField] float timeBetween = 0.2f;
    [SerializeField] int numShots = 5;
    Animator anim;
    AudioSource audioSource;
    PlayerStats stats;
    ProjectileSource shootSource;
    int shotCounter = 0;
    //[SerializeField] GameObject projectilePrefab;
    //[SerializeField] int baseProjectiles;   // This is the base amount of projectiles that exists with this ability
    //int numProjectiles; // This is the actual amount of projectiles shot with this ability
    
    public float damageMultipler = 1.0f;
    float damage;
    #endregion
    public override void InitAbility(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        audioSource = actor.GetComponent<AudioSource>();
        stats = actor.GetComponent<PlayerStats>();
        shootSource = actor.GetComponent<ProjectileSource>();

        if (stats.Projectiles.Value > 0)
        {
            shootSource.InitFirePositions((int)stats.Projectiles.Value);
        }
    }
    protected override void Fire(Ability ab, GameObject actor)
    {
        // Fire # projectiles, but how does this differentiate from hyper barrage
        // Hyper barrage could potentially launch a sequenece of the given numbers in the cone?

        // Calculate damage
        damage = stats.Damage.Value * damageMultipler * stats.DEX.Value;
    }

    // want ot use an enumerator, but cannot because it is a scriptable object...

    public override void OnTick()
    {
        // every tick, want to fire out a projectile until num projectiles reached.
        for (int i = 0; i < stats.Projectiles.Value; i++)
        {
            Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.ARROW_PROJECTILE, i);
            if (p != null)
            {
                p.SetDamage(damage);
                p.FireProjectile();
            }
            shotCounter++;
            if (shotCounter > numShots)
            {
                // once num num shots is reached, consume mana.
                shotCounter = 0;
                stats.Mana.Value -= cost;
            }
        }
    }
}
