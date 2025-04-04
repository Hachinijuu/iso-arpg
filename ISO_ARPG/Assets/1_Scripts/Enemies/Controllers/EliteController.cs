using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteController : EnemyControllerV2
{
    public override void Attack()
    {
        // Standard, melee attack
        base.Attack();
    }

    public virtual void RangedAttack()
    {
        // Standard, ranged attack
    }

    public virtual void SpecialAttack()
    {
        // Special, volley launcher, red indicated attack
    }
}
