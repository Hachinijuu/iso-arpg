using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Ability
{
    Animator anim;
    AudioSource source;
    ProjectileSource shootSource;
    public float damageMultipler = 1.0f;
    float damage;

    private int animId = Animator.StringToHash("Ability1");
    public override void InitAbility(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        source = actor.GetComponent<AudioSource>();
        shootSource = actor.GetComponent<ProjectileSource>();
    }

    public override void EndAbility()
    {
        base.EndAbility();
    }
    protected override void Fire(Ability ab, GameObject actor)
    {
        throw new System.NotImplementedException();
    }
}
