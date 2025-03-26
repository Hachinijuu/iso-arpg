using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "HyperBarrage", menuName = "sykcorSystems/Abilities/Hunter/HyperBarrage", order = 4)]
public class HyperBarrage : ChannelAbility
{
    // this is a fake passive ability, use it for the ticking functionality
    #region VARIABLES
    [SerializeField] float arrowOffset = 2.0f;
    [SerializeField] float timeBetween = 0.2f;
    [SerializeField] int numShots = 5;
    Animator anim;
    AudioSource audioSource;
    PlayerStats stats;
    ProjectileSource shootSource;
    PlayerParticleHandler particles;
    //int shotCounter = 0;    
    public float damageMultipler = 1.0f;
    private int animID = Animator.StringToHash("Ability2");
    float damage;
    #endregion
    public override void InitAbility(AbilityEventArgs e)
    {
        anim = e.Actor.Animator;
        audioSource = e.Actor.SFXSource;
        stats = e.Actor.Stats;
        shootSource = e.Actor.ShootSource;
        particles = e.Actor.Particles;

        if (stats.Projectiles.Value > 0)
        {
            shootSource.InitFirePositions((int)stats.Projectiles.Value);
        }
    }
    protected override void Fire()
    {
        // Calculate damage
        damage = stats.Damage.Value + damageMultipler * (stats.DEX.Value * GameManager.Instance.MainConvert);
        GameManager.Instance.CoroutineStarter(BarrageSequence());
        
        //BarrageSequence();  // This will not work
        //HandleCycle();
    }

    bool finishedSequence = true;

    // want ot use an enumerator, but cannot because it is a scriptable object...

    // Try conga-lining the barrage, or have a method to call the coroutine
    // Call barrage sequence, but need a monobehaviour class...
    private IEnumerator BarrageSequence()
    {
        if (finishedSequence)
        {
            finishedSequence = false;
            if (abilityActivated)
                audioSource.PlayOneShot(abilityActivated);

            particles.HandleAbility2Particles(true);
            anim.SetTrigger(animID);
            // Fire out five projectiles in succession

            for (int shots = 0; shots < numShots; shots++)
            {
                for (int pos = 0; pos < stats.Projectiles.Value; pos++)
                {
                    Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.ARROW_PROJECTILE, pos);
                    if (p != null)
                    {
                        p.transform.position += p.transform.forward * arrowOffset * shots;
                        p.SetDamage(damage);
                        p.FireProjectile();
                    }
                }
                yield return new WaitForSeconds(timeBetween); //shootSource.WaitForTime(timeBetween);
            }
            stats.Mana.Value -= cost;
            finishedSequence = true;
        }
    } 
    public void HandleCycle()
    {
        // for (int i = 0; i < stats.Projectiles.Value; i++)
        // {
        //     for (int shots = 0; shots < numShots; shots++)
        //     {
        //         Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.ARROW_PROJECTILE, i);
        //         if (p != null)
        //         {
        //             p.SetDamage(damage);
        //             p.FireProjectile();
        //         }
        //         // Add a wait between
        //         WaitForTime();
        //         // How to do a wait?
        //     }
        // }
    }
    
    public override void OnTick()
    {
        Fire();
        // every tick, want to fire out a projectile until num projectiles reached.
        //HandleCycle(); // Continue firing on ticks
        // for (int i = 0; i < stats.Projectiles.Value; i++)
        // {
        //     Projectile p = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.ARROW_PROJECTILE, i);
        //     if (p != null)
        //     {
        //         p.SetDamage(damage);
        //         p.FireProjectile();
        //     }
        //     shotCounter++;
        //     if (shotCounter > numShots)
        //     {
        //         stats.ID_Bar.Value += stats.IDGain.Value;
        //         // once num num shots is reached, consume mana.
        //         shotCounter = 0;
        //         stats.Mana.Value -= cost;
        //         // If more mana exists, possible to play shot continue sound here to indicate continous barrage
        //     }
        // }
    }

    public override void EndAbility()
    {
        particles.HandleAbility2Particles(false);
        base.EndAbility();
        //anim.SetBool(animID, false);
    }
}
