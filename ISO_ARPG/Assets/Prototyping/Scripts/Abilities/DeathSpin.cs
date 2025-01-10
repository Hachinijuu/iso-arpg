using UnityEngine;

[CreateAssetMenu(fileName = "DeathSpin", menuName = "sykcorSystems/Abilities/Berserker/DeathSpin", order = 4)]
public class DeathSpin : ChannelAbility
{
    Animator anim;
    PlayerMovement move;
    protected override void Fire(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        move = actor.GetComponent<PlayerMovement>();

        if (anim != null)
        {
            anim.SetBool("Ability2", true);
        }

        if (move != null)
        {
            anim.SetFloat("Speed", 0.0f);   // Set speed to none to return to idle
            move.UseAnimations = false;     // Stop the move animations from being used (override with spin)
            move.CanRotate = false;         // Stop player rotation
        }
    }

    public override void EndAbility(GameObject actor)
    {
        base.EndAbility(actor);
        anim.SetBool("Ability2", false);
        move.UseAnimations = true;
        move.CanRotate = true;
    }
}
