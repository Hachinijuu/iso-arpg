using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is adapted and modified from the FSM implementation class available on UnifyCommunity website
/// The license for the code is Creative Commons Attribution Share Alike.
/// It's originally the port of C++ FSM implementation mentioned in Chapter01 of Game Programming Gems 1
/// You're free to use, modify and distribute the code in any projects including commercial ones.
/// Please read the link to know more about CCA license @http://creativecommons.org/licenses/by-sa/3.0/
/// </summary>

public enum Transition
{
    None = 0,
    ChasePlayer,        // Agent to chase the player
    PlayerReached,      // Player has reached the agent
    ReachPlayer,        // Agent has reached the player
    NoHealth,           // Agent has no health remaining
}
public enum FSMStateID
{
    None = 0,
    Chase,
    MeleeAttack,
    RangedAttack,
    Regenerating,
    Dead,
}

public class AdvancedFSM : FSM
{
    private List<FSMState> fsmStates;

    // The fsmStates are not changing directly but updating using transitions
    private FSMStateID currentStateID;
    public FSMStateID CurrentStateID { get { return currentStateID; } }

    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }

    public AdvancedFSM()
    {
        fsmStates = new List<FSMState>();
    }

    //ADD NEW STATE INTO THE LIST
    public void AddFSMState(FSMState fsmState)
    {
        if (fsmStates == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }

        //First State inserted is also the Initial state
        //the state machine is in when the simulation begins

        if (fsmStates.Count == 0)
        {
            fsmStates.Add(fsmState);
            currentState = fsmState;
            currentStateID = fsmState.ID;
            return;
        }
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == fsmState.ID)
            {
                Debug.LogError("FSM ERROR: Trying to add a state that was already inside the list");
                return;
            }
        }
        // if no state is in the current then add the state to the list
        fsmStates.Add(fsmState);

    }
    /// <summary>
    /// This method delete a state from the FSM List if it exists, 
    ///   or prints an ERROR message if the state was not on the List.
    /// </summary>
    public void DeleteState(FSMStateID fsmState)
    {
        // Check for NullState before deleting
        if (fsmState == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: bull id is not allowed");
            return;
        }

        // Search the List and delete the state if it�s inside it
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == fsmState)
            {
                fsmStates.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: The state passed was not on the list. Impossible to delete it");
    }
    // <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn�t have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
    public void PerformTransition(Transition trans)
    {
        // Check for NullTransition before changing the current state
        if (trans == Transition.None)
        {
            Debug.LogError("FSM ERROR: Null transition is not allowed");
            return;
        }
        //Check if the currentState has the transition passed as an argument
        FSMStateID id = currentState.GetOutputState(trans);
        if (id == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: Current State does not have a target state for this transition");
            return;
        }
        //Update the currentStateID and the currentState
        currentStateID = id;
        foreach (FSMState state in fsmStates)
        {
            if (state.ID == currentStateID)
            {
                currentState = state;
                currentState.EnterStateInit();
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        switch(currentStateID)
        {
            case FSMStateID.Chase:
                Gizmos.color = Color.yellow;
            break;
            case FSMStateID.MeleeAttack:
                Gizmos.color = Color.red;
            break;
            case FSMStateID.RangedAttack:
                Gizmos.color = Color.blue;
            break;
        }
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
