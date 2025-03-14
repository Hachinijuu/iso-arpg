using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StateContainer", menuName = "sykcorSystems/AI/StateContainer", order = 0)]
public class StateContainer : ScriptableObject
{
    public EntityID entityId;
    // This may not work if dictionaries cannot be serialized
    // If they cannot, create a custom class for the keyvalue pair, and then unload the states into dictionaries inside the enemy controller
    // This may not be as resource intensive as the old controller fsm, because the dictionary simply references to the list of static states defined by this container class
    public Dictionary<AIState.StateId, AIState> stateMap;
    public List<StateLink> states;
    public void LoadStates()
    {
        if (stateMap == null)
            stateMap = new Dictionary<AIState.StateId, AIState>();

        stateMap.Clear();
        foreach (StateLink link in states)
        {
            stateMap.Add(link.id, link.state);
        }
    }
}

[System.Serializable]
public class StateLink
{
    public AIState.StateId id;
    public AIState state;
}
