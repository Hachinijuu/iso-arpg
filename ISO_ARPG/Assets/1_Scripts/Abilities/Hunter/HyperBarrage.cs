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
    public float damageMultipler = 1.0f;
    private int animID = Animator.StringToHash("Ability2");

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
        if (abilityActivated)
            audioSource.PlayOneShot(abilityActivated);

        anim.SetBool(animID, true);

        // Calculate damage
        damage = stats.Damage.Value * damageMultipler * (stats.DEX.Value * GameManager.Instance.MainConvert);
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
                stats.ID_Bar.Value += stats.IDGain.Value;
                // once num num shots is reached, consume mana.
                shotCounter = 0;
                stats.Mana.Value -= cost;
                // If more mana exists, possible to play shot continue sound here to indicate continous barrage
            }
        }
    }

    public override void EndAbility()
    {
        anim.SetBool(animID, false);
    }
}
