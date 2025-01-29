using UnityEngine;

[CreateAssetMenu(fileName = "RagingSwing", menuName = "sykcorSystems/Abilities/Berserker/RagingSwing", order = 4)]
public class RagingSwing : StackAbility
{
    Animator anim;
    AudioSource source;
    Hitbox[] hitboxes;
    PlayerStats stats;
    protected override void Fire(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        source = actor.GetComponent<AudioSource>();
        stats = actor.GetComponent<PlayerStats>();
        hitboxes = actor.GetComponentsInChildren<Hitbox>();

        if (anim != null)
        { 
            anim.SetTrigger("Ability1");
        }

        if (source != null)
        { 
            source.PlayOneShot(abilityActivated);
        }

        if (hitboxes != null)
        {
            SetDamageDetection(true);
        }

        if (stats != null)
        {
            // Raging Swing affects movespeed and attackspeed
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
        }

        //Debug.Log("Raging Swing");
        //throw new System.NotImplementedException();
    }

    public override void EndAbility(GameObject actor)
    {
        SetDamageDetection(false);

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
