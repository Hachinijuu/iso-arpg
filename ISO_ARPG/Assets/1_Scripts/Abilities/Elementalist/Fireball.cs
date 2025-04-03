using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "sykcorSystems/Abilities/Elementalist/Fireball", order = 4)]
public class Fireball : Ability
{
    Animator anim;
    AudioSource source;
    PlayerStats stats;
    ProjectileSource shootSource;
    public float damageMultipler = 1.0f;
    float damage;

    private int animId = Animator.StringToHash("Ability1");
    public override void InitAbility(AbilityEventArgs e)
    {
        anim = e.Actor.Animator;
        source = e.Actor.SFXSource;
        stats = e.Actor.Stats;
        shootSource = e.Actor.ShootSource;
    }
    protected override void Fire(ref AbilityEventArgs e)
    {
        if (abilityActivated)
            source.PlayOneShot(abilityActivated);
        
        anim.SetTrigger(animId);
        //stats.ID_Bar.Value += stats.IDGain.Value;

        damage = (stats.Damage.Value * damageMultipler) + (stats.INT.Value * GameManager.Instance.MainConvert);
        for (int i = 0; i < stats.Projectiles.Value; i++)
        {
            Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.FIREBALL, i);
            if (p != null)
            {
                e.Hitboxes.Add(p);
                p.SetDamage(damage);
                p.FireProjectile();
            }
        }
        PlayerManager.Instance.SetGiveDamage(true);
    }

    public override void EndAbility()
    {
        PlayerManager.Instance.SetGiveDamage(false);
        base.EndAbility();
    }
}
