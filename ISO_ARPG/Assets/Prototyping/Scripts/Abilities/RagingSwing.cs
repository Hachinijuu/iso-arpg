using UnityEngine;

[CreateAssetMenu(fileName = "RagingSwing", menuName = "sykcorSystems/Abilities/Berserker/RagingSwing", order = 4)]
public class RagingSwing : Ability
{
    Animator anim;
    AudioSource source;
    Hitbox[] hitboxes;
    protected override void Fire(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();
        source = actor.GetComponent<AudioSource>();
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
