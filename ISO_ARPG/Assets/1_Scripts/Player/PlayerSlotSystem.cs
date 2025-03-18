using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    private Dictionary<Transform, EnemyControllerV2> occupiedSlots;
    public void OnEnable()
    {
        SetupSlots();
    }

    public void OnDisable()
    {
        CleanupSlots();
    }
    public void SetupSlots()
    {
        if (slots == null || slots.Count <= 0) { return; }
        if (occupiedSlots != null) { return; }  // If the slot system has already been setup
        occupiedSlots = new Dictionary<Transform, EnemyControllerV2>();
        foreach (Transform pos in slots)
        {
            occupiedSlots.Add(pos, null); // False because the slot is not occupied
        }
        Debug.Log("[PlayerSlotSystem]: Setup Player Slots");
    }

    public void CleanupSlots()
    {
        foreach(Transform pos in slots)
        {
            occupiedSlots[pos] = null;
        }
    }
    public void ReserveSlot(EnemyControllerV2 agent)    // If an agent fails to reserve a slot when they try, then do nothing
    {
        Debug.Log("trying to reserve a slot");
        Transform slot = GetNearestSlot(agent.transform);
        if (slot == null) { return; }
        if (occupiedSlots.TryGetValue(slot, out EnemyControllerV2 found))
        {
            if (found == null)
            {
                occupiedSlots[slot] = agent;
                Debug.Log("reserved a slot");
            }
        }
        //if (occupiedSlots[slot] != null)    // If the nearest slot found is not occupied
        //{
        //    occupiedSlots[slot] = agent;    // Assign the slot to the agent
        //}
    }

    public bool CheckHasSlot(EnemyControllerV2 agent)
    {
        if (occupiedSlots.ContainsValue(agent)) // If the dictionary has the agent
            return true;    // The agent DOES have a slot
        else
            return false;   // Otherwise, the agent does not
    }
    public void UnreserveSlot(EnemyControllerV2 agent)
    {
        // Only unreserve the slot if the agent does have the slot
        if (CheckHasSlot(agent))
        {
            // Remove the stored reference
            for (int i = 0; i < occupiedSlots.Count; i++)
            {
                Transform key = slots[i];
                if (occupiedSlots.TryGetValue(key, out EnemyControllerV2 slotAgent))
                {
                    if (slotAgent == agent)
                    {
                        Debug.Log("Matching agent");
                        occupiedSlots[key] = null;  // Remove the reference
                        return; // return to end the loop early
                    }
                }
            }
        }
    }
    public Transform GetNearestSlot(Transform pos)
    {
        Transform nearest = null;
        float nearestDist = float.MaxValue;

        // Get the distance between the request position and the slots, return the nearest slot
        foreach (KeyValuePair<Transform, EnemyControllerV2> agentSlot in occupiedSlots)
        {
            //if (agentSlot.Value != null) { continue; } // If the slot is already occupied, continue to the next slot
            // If the slot is NOT occupied
            if (agentSlot.Value == null)
            {
                float dist = Vector3.Distance(agentSlot.Key.position, pos.position);    // Check the distance between the agent and the slot
                //Debug.Log(dist);
                if (dist < nearestDist) // If the distance is less than the nearest, it is the new nearest distance
                {
                    nearestDist = dist;
                    nearest = agentSlot.Key;
                    // Return the slot as a valid position to take (Unoccupied)
                    //Debug.Log("Shortest Distance");
                }
            }
            // /return agentSlot.Key;
        }
        return nearest;
    }

    public Transform GetSlot(EnemyControllerV2 agent)
    {
        if (CheckHasSlot(agent))
        {
            // Get the slot
            for (int i = 0; i < occupiedSlots.Count; i++)
            {
                Transform key = slots[i];
                if (occupiedSlots.TryGetValue(key, out EnemyControllerV2 slotAgent))
                {
                    if (slotAgent == agent)
                        return key;
                }
            }
        }
        return null;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos() 
    {
        if (occupiedSlots == null || occupiedSlots.Count <= 0 ) { return; }
        foreach (KeyValuePair<Transform, EnemyControllerV2> slot in occupiedSlots)
        {
            if (slot.Value != null) // if the slot is occupied
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.white;
            }
            Gizmos.DrawWireSphere(slot.Key.position, 0.25f);
        }
    }
    #endif
}
