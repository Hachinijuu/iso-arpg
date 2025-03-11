using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollection", menuName = "sykcorSystems/Sounds/SoundHolder", order = 2)]
public class SoundHolder : ScriptableObject
{
    public AudioClip[] sounds;
}
