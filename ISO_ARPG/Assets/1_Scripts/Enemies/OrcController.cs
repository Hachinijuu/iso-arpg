using UnityEngine;
public class OrcController : EnemyController
{
    protected override void ConstructFSM()
    {
        DeadState dead = new DeadState(this);

        ChaseState chase = new ChaseState(this);
        chase.AddTransistion(Transition.PlayerReached, FSMStateID.MeleeAttack);
        chase.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        MeleeAttackState melee = new MeleeAttackState(this);
        melee.AddTransistion(Transition.ChasePlayer, FSMStateID.Chase);
        melee.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        AddFSMState(chase);
        AddFSMState(melee);
        AddFSMState(dead);
    }
}