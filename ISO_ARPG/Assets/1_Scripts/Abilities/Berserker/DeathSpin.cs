using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathSpin", menuName = "sykcorSystems/Abilities/Berserker/DeathSpin", order = 4)]
public class DeathSpin : ChannelAbility
{
    #region VARIABLES
    Animator anim;
    AudioSource source;
    PlayerMovement move;
    Hitbox[] hitboxes;
    PlayerStats stats;
    ProjectileSource shootSource;
    PlayerParticleHandler particles;
    public float damageMultipler = 1.0f;
    float damage;

    // EACH ABILITY IS UNIQUE, BUT IS IT A GOOD PRACTICE TO GET THESE VALUES UNIQUELY -- SO ASSET SO IT MIGHT BE FINE EXISTING AS UNIQUE VALUES

    private int moveAnimID = Animator.StringToHash("Speed");
    private int abilAnimID = Animator.StringToHash("Ability2");     // perhaps expose the slot, and then pass string to slot
    // OR, leave abilities hardcoded, most characters will be custom built regardless in terms of animatior setups and masks

    #endregion
    #region FUNCTIONALITY
    public override void InitAbility(AbilityEventArgs e)
    {
        anim = e.Actor.Animator;
        source = e.Actor.SFXSource;
        hitboxes = e.Actor.hitboxes; //e.Actor.GetComponentsInChildren<Hitbox>();
        stats = e.Actor.Stats;
        move = e.Actor.Movement;
        shootSource = e.Actor.ShootSource;
        particles = e.Actor.Particles;
    }
    protected override void Fire()
    {
        damage = stats.Damage.Value + damageMultipler * (stats.STR.Value * GameManager.Instance.MainConvert);
        anim.SetBool(abilAnimID, true);

        if (abilityActivated)
        {
            source.clip = abilityActivated;
            source.loop = true;
            source.Play();
        }
        SetDamageDetection(true);

        // Using the shootSource, rotate the fire location and use this to handle the positional shots
        // Rotate towards a targetted enemy (Target selection is via click)
        // Can continue the movemennt, but shots will be directed towards the target

        anim.SetFloat(moveAnimID, 0.0f);   // Set speed to none to return to idle

        // Only want this ability to drive movement IF the user is using click to move (current settings are bound locally, final settings will be centralized)
        if (move.type == MoveInput.CLICK)
        {
            move.MoveHeld = true;           // Allow this to drive movement
        }
        move.UseAnimations = false;     // Stop the move animations from being used (override with spin)
        move.CanRotate = false;         // Stop player rotation
        particles.HandleAbility2Particles(true);
    }
    public override void EndAbility(AbilityEventArgs e)
    {
        anim.SetBool(abilAnimID, false);
        SetDamageDetection(false);

        source.clip = null;
        source.loop = false;
        source.Stop();
        if (move.type == MoveInput.CLICK)
        {
            move.MoveHeld = false;           // Allow this to drive movement
        }
        move.UseAnimations = true;
        move.CanRotate = true;

        particles.HandleAbility2Particles(false);
        base.EndAbility(e);
    }

    public override void OnTick()
    {
        //stats.ID_Bar.Value += stats.IDGain.Value;

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
    }

    #region HELPER FUNCTIONS
    void SetDamageDetection(bool on)
    {
        //float damageToDeal = stats.Damage.Value * damageMultipler * stats.STR.Value;
        if (hitboxes != null && hitboxes.Length > 0)
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].ApplyDamage = on;
                hitboxes[i].SetDamage(damage);
                hitboxes[i].OpenHitWindow();
            }
        }
    }
    #endregion
    #endregion
}
