using UnityEngine;

[CreateAssetMenu(fileName = "RagingSwing", menuName = "sykcorSystems/Abilities/Berserker/RagingSwing", order = 4)]
public class RagingSwing : Ability
{
    Animator anim;
    protected override void Fire(Ability ab, GameObject actor)
    {
        anim = actor.GetComponent<Animator>();

        anim.SetTrigger("Ability1");
        //Debug.Log("Raging Swing");
        //throw new System.NotImplementedException();
    }
}
