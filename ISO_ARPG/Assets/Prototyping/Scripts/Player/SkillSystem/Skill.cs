using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    // unused for now, implemented for later testing of state swaps
    //protected enum skillStates { INVALID, INACTIVE, ACTIVE, COOLDOWN };
    //skillStates currState = skillStates.INVALID;

    public string Name { get { return skName; } }
    public string Description { get { return skDescription; } }

    public float Cooldown { get { return cooldown; } }
    public float ActiveTime { get { return activeTime; } }

    [SerializeField]
    protected string skName = "invalid";

    [SerializeField]
    protected string skDescription = "invalid";

    [SerializeField]
    protected float cooldown = -1f;

    [SerializeField]
    protected float activeTime = -1f;

    protected bool used = false;


    // call this function in the input handler, if the user can use their skill it will be used, otherwise output that it cannot be used.
    public virtual void UseSkill()
    {
        if (!used)
        { 
            SkillAction();
            StartCoroutine(WaitCooldown());
        }
        else
            Debug.Log("Cannot use the ability");
    }

    IEnumerator WaitCooldown()
    {
        used = true;
        yield return new WaitForSeconds(cooldown);
        used = false;
    }
    protected abstract void SkillAction();   // write the functionality of the skill here in the derived classes
}
