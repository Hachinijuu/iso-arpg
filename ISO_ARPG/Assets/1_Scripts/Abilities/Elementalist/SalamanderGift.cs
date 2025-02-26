using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalamanderGift : IdentityAbility
{
    AudioSource source;
    PlayerStats stats;
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
}
