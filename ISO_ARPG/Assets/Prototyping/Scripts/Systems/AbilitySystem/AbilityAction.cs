using UnityEngine;

public abstract class AbilityAction : MonoBehaviour
{
    public abstract void Fire(Ability ability, GameObject actor); // Write the action of this ability within this function.
}
