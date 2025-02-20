using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] bool promptInteraction;
    [SerializeField] GameObject interactPrompt;
    public int interactDistance = 5;
    public virtual void OnInteract(Transform source = null)
    {
        // only check for distance if source != null and distance > 0
        if (source != null && interactDistance > 0)
        {
            if (!IsInCurrentRange(source.position, transform.position, interactDistance))
            {
                return; // If you need to check for range, and you're not in range, return
            }
        }

        if (!interactPrompt)
            InteractAction();
        else
            interactPrompt.SetActive(true); // prompt needs to handle interact action call
    }

        public virtual void OnInteract()
    {
        if (!interactPrompt)
            InteractAction();
        else
            interactPrompt.SetActive(true); // prompt needs to handle interact action call
    }

    protected virtual bool IsInCurrentRange(Vector3 start, Vector3 goal, int range)
    {
        bool inRange = false;
        //float dist = Vector3.Distance(trans.position, pos);
        // Vector3.Distance is the same as (a-b).magnitude -- instead, use squared magnitudes so square root is not used

        // Transform is the goal, pos is the start
        float dist = (goal - start).sqrMagnitude;

        if (dist <= (range * range))
        {
            inRange = true;
        }
        return inRange;
    }

    protected abstract void InteractAction();
}
