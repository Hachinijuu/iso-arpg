using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is adapted and modified from the FSM implementation class available on UnifyCommunity website
/// The license for the code is Creative Commons Attribution Share Alike.
/// It's originally the port of C++ FSM implementation mentioned in Chapter01 of Game Programming Gems 1
/// You're free to use, modify and distribute the code in any projects including commercial ones.
/// Please read the link to know more about CCA license @http://creativecommons.org/licenses/by-sa/3.0/

/// This class represents the States in the Finite State System.
/// Each state has a Dictionary with pairs (transition-state) showing
/// which state the FSM should be if a transition is fired while this state
/// is the current state.
/// Reason method is used to determine which transition should be fired .
/// Act method has the code to perform the actions the NPC is supposed to do if it큦 on this state.
/// </summary>

public abstract class FSMState
{
    protected Dictionary<Transition, FSMStateID> map = new Dictionary<Transition, FSMStateID>();
    protected FSMStateID stateID;
    public FSMStateID ID { get { return stateID; } }
    protected Vector3 desPos;
    protected float curSpeed;

    public void AddTransistion(Transition transition, FSMStateID id)
    {
        //Chek if anyone of the args is invalid
        if (transition == Transition.None || id == FSMStateID.None)
        {
            Debug.LogWarning("FSMState: Null transition not allowed");
            return;
        }
        map.Add(transition, id);
        Debug.Log("Added: " + transition + " with ID: " + id);

    }

    /// This method deletes a pair transition-state from this state큦 map.
    /// If the transition was not inside the state큦 map, an ERROR message is printed.

    public void DeleteTransition(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }

        // Check if the pair is inside the map before deleting
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition passed was not on this State큦 List");
    }

    /// This method returns the new state the FSM should be if
    ///    this state receives a transition  

    public FSMStateID GetOutputState(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return FSMStateID.None;
        }

        // Check if the map has this transition
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }

        Debug.LogError("FSMState ERROR: " + trans + " Transition passed to the State was not on the list");
        return FSMStateID.None;
    }

    // Used to initialize variables when re-entering state
    public virtual void EnterStateInit()
    {

    }

    /// <summary>
    /// Decides if the state should transition to another on its list
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Reason(Transform player, Transform npc);

    /// <summary>
    /// This method controls the behavior of the NPC in the game World.
    /// Every action, movement or communication the NPC does should be placed here
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Act(Transform player, Transform npc);

    /// <summary>
    /// Check whether the next random position is the same as current tank position
    /// </summary>
    /// <param name="pos">position to check</param>
    protected virtual bool IsInCurrentRange(Transform trans, Vector3 pos, int range)
    {
        bool inRange = false;
        float dist = Vector3.Distance(trans.position, pos);
        if (dist <= range)
        {
            inRange = true;
        }
        return inRange;
    }

}
