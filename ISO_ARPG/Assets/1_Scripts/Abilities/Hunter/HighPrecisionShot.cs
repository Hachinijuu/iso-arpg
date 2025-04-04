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
    private int animID = Animator.StringToHash("Ability1");

    #endregion

    public override void InitAbility(AbilityEventArgs e)
    {
        anim = e.Actor.Animator;
        audioSource = e.Actor.SFXSource;
        stats = e.Actor.Stats;
        shootSource = e.Actor.ShootSource;
    }
    protected override void Fire(ref AbilityEventArgs e)
    {
        if (abilityActivated)
            audioSource.PlayOneShot(abilityActivated);

        anim.SetTrigger(animID);
        //stats.ID_Bar.Value += stats.IDGain.Value;

        // Calculate damage
        damage = (stats.Damage.Value * stats.SecondaryDamage.Value) + (stats.DEX.Value * GameManager.Instance.MainConvert);
        for (int i = 0; i < stats.Projectiles.Value; i++)
        {
            Projectile projectile = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.ARROW_PROJECTILE, i);
            if (projectile != null)
            {
                e.Hitboxes.Add(projectile);
                projectile.SetDamage(damage);
                projectile.FireProjectile();
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
