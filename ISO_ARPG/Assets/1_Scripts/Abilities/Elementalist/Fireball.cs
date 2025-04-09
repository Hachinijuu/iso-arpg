using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "sykcorSystems/Abilities/Elementalist/Fireball", order = 4)]
public class Fireball : Ability
{
    Animator anim;
    AudioSource source;
    PlayerStats stats;
    ProjectileSource shootSource;
    float damageUptime;
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

        damage = stats.Damage.Value * (1 + (stats.PrimaryDamage.Value / 100)) + (stats.INT.Value * GameManager.Instance.MainConvert);
        for (int i = 0; i < stats.Projectiles.Value; i++)
        {
            Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.FIREBALL, i);
            if (p != null)
            {
                e.Hitboxes.Add(p);
                damageUptime = p.uptime;
                p.SetDamage(damage);
                p.InitHitbox(stats);
                p.FireProjectile();
            }
        }
        PlayerManager.Instance.SetGiveDamage(true, damageUptime);
    }

    public override void EndAbility(AbilityEventArgs e)
    {
        PlayerManager.Instance.SetGiveDamage(false, damageUptime);
        base.EndAbility(e);
    }
}
