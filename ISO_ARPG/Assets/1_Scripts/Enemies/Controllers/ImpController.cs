using UnityEngine;
public class ImpController : EnemyController
{
    protected override void ConstructFSM()
    {
        DeadState dead = new DeadState(this);

        ChaseState chase = new ChaseState(this);
        chase.AddTransistion(Transition.ReachPlayer, FSMStateID.RangedAttack);
        chase.AddTransistion(Transition.PlayerReached, FSMStateID.MeleeAttack);
        chase.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        MeleeAttackState melee = new MeleeAttackState(this);
        melee.AddTransistion(Transition.ChasePlayer, FSMStateID.Chase);
        melee.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        RangedAttackState ranged = new RangedAttackState(this);
        ranged.AddTransistion(Transition.ChasePlayer, FSMStateID.Chase);
        ranged.AddTransistion(Transition.ReachPlayer, FSMStateID.MeleeAttack);
        ranged.AddTransistion(Transition.NoHealth, FSMStateID.Dead);

        AddFSMState(chase);
        AddFSMState(melee);
        AddFSMState(ranged);
        AddFSMState(dead);
    }

    public override void Respawn()
    {
        base.Respawn();
        OverrideState(FSMStateID.Chase);
    }
}