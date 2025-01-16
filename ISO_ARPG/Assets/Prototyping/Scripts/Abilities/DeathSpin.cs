using UnityEngine;

[CreateAssetMenu(fileName = "DeathSpin", menuName = "sykcorSystems/Abilities/Berserker/DeathSpin", order = 4)]
public class DeathSpin : ChannelAbility
{
    Animator anim;
    AudioSource source;
    PlayerMovement move;
    Hitbox[] hitboxes;

    protected override void Fire(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        source = actor.GetComponent<AudioSource>();
        hitboxes = actor.GetComponentsInChildren<Hitbox>();
        move = actor.GetComponent<PlayerMovement>();

        //Debug.Log(hitboxes.Length);
        if (anim != null)
        {
            anim.SetBool("Ability2", true);
        }
        if (source != null)
        {
            source.clip = abilityActivated;
            source.loop = true;
            source.Play();
        }

        if (hitboxes != null)
        {
            SetDamageDetection(true);
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
        SetDamageDetection(false);

        source.clip = null;
        source.loop = false;
        source.Stop();
        if (move.moveType == PlayerMovement.MoveInput.CLICK)
        {
            move.MoveHeld = false;           // Allow this to drive movement
        }
        move.UseAnimations = true;
        move.CanRotate = true;
    }

    void SetDamageDetection(bool on)
    {
        if (hitboxes != null && hitboxes.Length > 0)
        {
            for (int i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].ApplyDamage = on;
            }
        }
    }
}
