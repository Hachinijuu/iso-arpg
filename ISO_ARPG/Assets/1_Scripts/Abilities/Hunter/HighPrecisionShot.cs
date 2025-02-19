using UnityEngine;

[CreateAssetMenu(fileName = "HighPrecisionShot", menuName = "sykcorSystems/Abilities/Hunter/HighPrecisionShot", order = 4)]
public class HighPrecisionShot : Ability
{
    #region VARIABLES
    Animator anim;
    AudioSource audioSource;
    PlayerStats stats;
    ProjectileSource shootSource;
    public float damageMultipler = 1.0f;
    float damage;

    #endregion

    public override void InitAbility(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        audioSource = actor.GetComponent<AudioSource>();
        stats = actor.GetComponent<PlayerStats>();
        shootSource = actor.GetComponent<ProjectileSource>();
    }
    protected override void Fire(Ability ab, GameObject actor)
    {
        if (stats.Projectiles.Value > 0)
        {
            // If there are projectiles to shoot
            // Calculate damage
            damage = stats.Damage.Value * damageMultipler * stats.DEX.Value;
            for (int i = 0; i < stats.Projectiles.Value; i++)
            {
                Projectile projectile = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.ARROW_PROJECTILE, i);
                if (projectile != null)
                {
                    projectile.SetDamage(damage);
                    projectile.FireProjectile();
                }
            }
        }
    }
}
