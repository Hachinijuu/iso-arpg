using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenSpikes : Ability
{
    Animator anim;
    AudioSource source;
    PlayerStats stats;
    ProjectileSource shootSource;
    public float damageMultipler = 1.0f;
    float damage;
    private int animId = Animator.StringToHash("Ability2");
    public override void InitAbility(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        source = actor.GetComponent<AudioSource>();
        stats = actor.GetComponent<PlayerStats>();
        shootSource = actor.GetComponent<ProjectileSource>();
    }
    protected override void Fire(Ability ab, GameObject actor)
    {
        if (abilityActivated)
            source.PlayOneShot(abilityActivated);
        
        anim.SetTrigger(animId);
        stats.ID_Bar.Value += stats.IDGain.Value;

        damage = (stats.Damage.Value * damageMultipler) + (stats.INT.Value * GameManager.Instance.MainConvert);
        for (int i = 0; i < stats.Projectiles.Value; i++)
        {
            Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.FIREBALL, i);
            if (p != null)
            {
                p.SetDamage(damage);
                p.FireProjectile();

                // Fireballs will have the chain effect on them, which will handle the ability chaining
                // Perhaps adjust base hitbox class, and activate or deactivate chained damage accordingly
                
            }
        }
    }
}
