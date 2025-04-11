using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityAbility : PassiveAbility
{
    [SerializeField] public AudioClip altSound;
    public bool asFusion = false;

    // when an identity ability is used as a fusion, it has reduced values.
    // this acts as an interface of whether or not the identity ability should time out. -- fusions will not

    // WRAPPER CLASS
}
