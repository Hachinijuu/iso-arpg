using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootstepCollection", menuName = "sykcorSystems/Sounds/FootSteps", order = 1)]
public class FootStepSounds : ScriptableObject
{
    FloorMaterials material;
    public AudioClip[] steps;
}
