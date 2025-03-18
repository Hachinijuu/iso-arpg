using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class AICoroutineState : AIState
// {
//     [Header("Coroutine Settings")]
//     public float intervalTime = 1.0f;
//     public override void Reason(AgentStateArgs e)
//     {
//         //throw new System.NotImplementedException();
//     }
//     public override void Act(AgentStateArgs e)
//     {
//         //throw new System.NotImplementedException();
//     }
// }

// This interface adds the method, FixedAct(); for physics based movement handling
public interface IPhysicsState
{
    public void FixedAct(AgentStateArgs e);
}

// This interface adds interval time for coroutine based states, allow the state to be categorized easily
public interface ICoroutineState
{
    public abstract float intervalTime { get; set; }
}
