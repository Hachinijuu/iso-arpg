using UnityEngine;

[CreateAssetMenu(fileName = "RagingSwing", menuName = "sykcorSystems/Abilities/Berserker/RagingSwing", order = 4)]
public class RagingSwing : StackAbility
{
    #region VARIABLES
    Animator anim;
    AudioSource audioSource;
    Hitbox[] hitboxes;
    PlayerStats stats;
    ProjectileSource shootSource;
    public float damageMultipler = 1.0f;
    float damage;

    private int animId = Animator.StringToHash("Ability1");
    #endregion
    #region FUNCTIONALITY
    public override void InitAbility(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        audioSource = actor.GetComponent<AudioSource>();
        stats = actor.GetComponent<PlayerStats>();
        hitboxes = actor.GetComponentsInChildren<Hitbox>();
        shootSource = actor.GetComponent<ProjectileSource>();
    }
    protected override void Fire(Ability ab, GameObject actor)
    {
        // Call calculator on the relevant hitboxes ... kind of messy in terms of how projectiles run and apply damage
        // Generate damage values and then pass to init?

        damage = stats.Damage.Value * damageMultipler * stats.STR.Value;
        // Listen the the hitboxes for their events, if something was hit, regain mana - if nothing is hit, don't

        if (stats.Projectiles.Value > 0)
        {
            // If there are projectiles to shoot
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

        anim.SetTrigger(animId);
        //if (anim != null)
        //{ 
        //}

        if (abilityActivated)
            audioSource.PlayOneShot(abilityActivated);
        //if (source != null)
        //{ 
        //}

        SetDamageDetection(true);
        //if (hitboxes != null)
        //{
        //}

        if (stacks < maxStacks)
        {
            stacks++;
            if (GetSubStatFromType(SubStatTypes.MOVE_SPEED) != null)
            {
                stats.MoveSpeed.Value = stats.MoveSpeed.Value + (CalculateStat(GetSubStatFromType(SubStatTypes.MOVE_SPEED)).Value);
            }
            if (GetSubStatFromType(SubStatTypes.ATTACK_SPEED) != null)
            {
                stats.AttackSpeed.Value = stats.AttackSpeed.Value + (CalculateStat(GetSubStatFromType(SubStatTypes.ATTACK_SPEED)).Value);
            }
        }
        else
            Debug.Log("[RagingSwing]: Max Stacked");

        // if (stats != null)
        // {
        //     // Raging Swing affects movespeed and attackspeed

        // }

        //Debug.Log("Raging Swing");
        //throw new System.NotImplementedException();
    }

    public override void EndAbility(GameObject actor)
    {
        SetDamageDetection(false);
    }

    #region HELPER FUNCTIONS
    void SetDamageDetection(bool on)
    {
        // Damage Calculation
        if (hitboxes != null && hitboxes.Length > 0)
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].AllowDamageForTime(0.75f);
                hitboxes[i].SetDamage(damage);
            }
        }
    }
    #endregion
    #endregion
}
