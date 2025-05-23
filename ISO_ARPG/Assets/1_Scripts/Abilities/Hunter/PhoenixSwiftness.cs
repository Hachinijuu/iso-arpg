using UnityEngine;

[CreateAssetMenu(fileName = "PhoenixSwiftness", menuName = "sykcorSystems/Abilities/Hunter/PhoenixSwiftness", order = 4)]
public class PhoenixSwiftness : IdentityAbility
{
    #region VARIABLES
    public int movespeedIncrease = 1;
    public int dodgeIncrease = 1;
    public int projectileIncrease = 1;

    AudioSource source;
    PlayerStats stats;
    PlayerParticleHandler particles;

    PlayerStats defaultCopy;
    #endregion
    #region FUNCTIONALITY
    public override void InitAbility(AbilityEventArgs e)
    {
        stats = e.Actor.Stats;
        source = e.Actor.SFXSource;
        particles = e.Actor.Particles;
        //defaultCopy = new PlayerStats();
        if (asFusion)
        {
            // stop the ability from being timed out, reduce the stat buff values to be expected
        }
    
    }

    protected override void Fire(ref AbilityEventArgs e)
    {
        if (stats != null)
        {
            if (abilityActivated)
            {
                if (PlayerManager.Instance.currentCharacter.Body == GoX_Body.ONE)
                {
                    source.PlayOneShot(abilityActivated);
                }
                else
                {
                    if (altSound != null)
                    {
                        source.PlayOneShot(altSound);
                    }
                }
            }

            // create a copy of the stats, to revert back to
            //defaultCopy.CopyFromStats(stats);

            // add the increased stats
            stats.MoveSpeed.Value += movespeedIncrease;         // Add * 1.2 multiplier to speed = 20% increase
            stats.Dodge.Value += dodgeIncrease;                 // Add 25% more dodge -- flat number
            stats.Projectiles.Value += projectileIncrease;      // Add # of projectiles
            Debug.Log("Added Stats to: " + stats);

            if (!asFusion)
            {
                particles.ActivateAura();
            }
        }
    }

    public override void EndAbility(AbilityEventArgs e)
    {
        // Remove stats
        stats.MoveSpeed.Value -= movespeedIncrease;         // Add * 1.2 multiplier to speed = 20% increase
        stats.Dodge.Value -= dodgeIncrease;                 // Add 25% more dodge -- flat number
        stats.Projectiles.Value -= projectileIncrease;      // Add # of projectiles

        if (!asFusion)
        {
            particles.DeactivateAura();
        }
        //stats.CopyFromStats(defaultCopy);
    }
    #endregion
}
