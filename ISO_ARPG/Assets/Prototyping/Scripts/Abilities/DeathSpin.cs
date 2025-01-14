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

            // Only want this ability to drive movement IF the user is using click to move (current settings are bound locally, final settings will be centralized)
            if (move.moveType == PlayerMovement.MoveInput.CLICK)
            {
                move.MoveHeld = true;           // Allow this to drive movement
            }
            move.UseAnimations = false;     // Stop the move animations from being used (override with spin)
            move.CanRotate = false;         // Stop player rotation
        }
    }

    public override void EndAbility(GameObject actor)
    {
        base.EndAbility(actor);
        anim.SetBool("Ability2", false);
        if (move.moveType == PlayerMovement.MoveInput.CLICK)
        {
            move.MoveHeld = false;           // Allow this to drive movement
        }
        move.UseAnimations = true;
        move.CanRotate = true;
    }
}
