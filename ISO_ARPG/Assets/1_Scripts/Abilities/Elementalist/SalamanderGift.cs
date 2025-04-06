using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SalamanderGift", menuName = "sykcorSystems/Abilities/Elementalist/SalamanderGift", order = 4)]
public class SalamanderGift : IdentityAbility
{
    AudioSource source;
    PlayerStats stats;
    PlayerParticleHandler particles;

    public float manaMultipler = 2.0f;
    public int dmgFromMana = 25;
    public override void InitAbility(AbilityEventArgs e)
    {
        stats = e.Actor.Stats;
        source = e.Actor.SFXSource;
        particles = e.Actor.Particles;

        //defaultCopy = new PlayerStats();
        if (asFusion)
        {
            // stop the ability from being timed out, reduce the stat buff values to be expected amounts
        }
    }

    protected override void Fire(ref AbilityEventArgs e)
    {
        if (abilityActivated)
            source.PlayOneShot(abilityActivated);

        stats.Mana.MaxValue *= manaMultipler;
        stats.Chains.Value += 1;
        stats.DamageFromMana.Value += dmgFromMana;

        if (!asFusion)
        {
            particles.ActivateAura();
        }
        // Damage taken as mana instead of health...

        // This will require a rework to the hurtbox input
    }

    public override void EndAbility(AbilityEventArgs e)
    {
        stats.Mana.MaxValue /= manaMultipler;
        stats.Chains.Value -= 1;
        stats.DamageFromMana.Value -= dmgFromMana;

        if (!asFusion)
        {
            particles.DeactivateAura();
        }
        base.EndAbility(e);
    }


}
