using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalamanderGift : IdentityAbility
{
    AudioSource source;
    PlayerStats stats;

    public float manaMultipler = 2.0f;
    public override void InitAbility(Ability ab, GameObject actor)
    {
        stats = actor.GetComponent<PlayerStats>();
        source = actor.GetComponent<AudioSource>();

        //defaultCopy = new PlayerStats();
        if (asFusion)
        {
            // stop the ability from being timed out, reduce the stat buff values to be expected amounts
        }
    }

    protected override void Fire(Ability ab, GameObject actor)
    {
        if (abilityActivated)
            source.PlayOneShot(abilityActivated);

        stats.Mana.MaxValue *= manaMultipler;
        stats.Chains.Value += 1;

        // Damage taken as mana instead of health...

        // This will require a rework to the hurtbox input
    }


}
