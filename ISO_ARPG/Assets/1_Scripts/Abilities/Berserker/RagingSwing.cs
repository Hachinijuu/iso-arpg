using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "RagingSwing", menuName = "sykcorSystems/Abilities/Berserker/RagingSwing", order = 4)]
public class RagingSwing : StackAbility
{
    #region VARIABLES
    Animator anim;
    AudioSource audioSource;
    Hitbox[] hitboxes;
    PlayerStats stats;
    ProjectileSource shootSource;
    PlayerParticleHandler particles;
    
    public float checkOnHitWindow = 1.5f;
    //public float damageMultipler = 1.0f;
    float damage;
    GameObject body;
    int animCounter;

    private int swing1 = Animator.StringToHash("Ability1_Left");
    private int swing2 = Animator.StringToHash("Ability1_Right");

    #endregion
    #region FUNCTIONALITY
    public override void InitAbility(AbilityEventArgs e)
    {
        anim = e.Actor.Animator;
        audioSource = e.Actor.SFXSource;
        stats = e.Actor.Stats;
        hitboxes = e.Actor.hitboxes; //e.Actor.transform.GetComponentsInChildren<Hitbox>();

        //foreach (Hitbox hb in hitboxes)
        //{
        //    e.Hitboxes.Add(hb);
        //}

        shootSource = e.Actor.ShootSource;
        particles = e.Actor.Particles;
        body = e.Actor.Body;
        animCounter = 0;

        foreach (Hitbox hb in hitboxes)
        {
            e.Hitboxes.Add(hb);
            hb.InitHitbox(stats);
        }

        shootSource.InitFirePositions();
    }
    protected override void Fire(ref AbilityEventArgs e)
    {
        // Call calculator on the relevant hitboxes ... kind of messy in terms of how projectiles run and apply damage
        // Generate damage values and then pass to init?

        damage = (stats.Damage.Value * stats.SecondaryDamage.Value) + (stats.STR.Value * GameManager.Instance.MainConvert);
        //Debug.Log(damage);
        //Debug.Log(stats.STR.Value * GameManager.Instance.MainConvert);
        // Listen the the hitboxes for their events, if something was hit, regain mana - if nothing is hit, don't

        if (stats.Projectiles.Value > 0)
        {
            // If there are projectiles to shoot
            for (int i = 0; i < stats.Projectiles.Value; i++)
            {
                Projectile projectile = shootSource.GetPooledProjectile(ObjectPoolManager.PoolTypes.AXE_PROJECTILE, i);
                if (projectile != null)
                {
                    projectile.SetDamage(damage);
                    projectile.FireProjectile();
                }
            }
        }

        // Is it possible to change the speed of the animation?
        // Can abilities have the animationClip?


        animCounter++;
        //Debug.Log("AnimStep: " + animCounter);

        if (animCounter == 1)
        {
            anim.SetTrigger(swing1);
        }
        else
        {
            anim.SetTrigger(swing2);
        }
        //anim.SetInteger(animId, animCounter);       // why does the animation not play -- toggle swap works
        //Debug.Log(animCounter);
        // figure out animation transition

        //anim.SetBool(animId, left);
        //left = !left;
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

        // Only want to return valid infomration on successful hits --> listen to the hitbox onhit, and apply effects on successful hits


        // For now, to stop things from breaking, simply give id gains and such
        //stats.ID_Bar.Value += stats.IDGain.Value;
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

        //particles.HandleAbility1Particles(true);
        particles.PlayParticlesFor(checkOnHitWindow);
        PlayerManager.Instance.SetGiveDamage(true, checkOnHitWindow);
    }

    public override void EndAbility(AbilityEventArgs e)
    {
        particles.HandleAbility1Particles(false);
        if (animCounter == 2)
        {
            animCounter = 0;
        }
        SetDamageDetection(false);
        PlayerManager.Instance.SetGiveDamage(false, 0.75f);
    }

    #region HELPER FUNCTIONS
    void SetDamageDetection(bool on)
    {
        // Damage Calculation
        if (hitboxes != null && hitboxes.Length > 0)
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].AllowDamageForTime(0.75f, true);
                hitboxes[i].SetDamage(damage);
            }
        }
    }
    #endregion
    #endregion
}
