using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorMaterials { GRASS, MARBLE }
public class FootStepHandler : MonoBehaviour
{
    public AudioSource stepSource;
    public FootStepSounds[] steps;
    FloorMaterials currMaterial;
    public FootStepSounds currStep;

    public void SetMaterial(FloorMaterials toSet)
    {
        currMaterial = toSet;
        currStep = steps[(int)currMaterial];    // The array of steps must line up for this to work properly
        // Set materials when loading between scenes, --> could later set dynamically, but this is a quick and dirty solution
    }
    public void PlayFootstep()
    {
        // Get the material --> don't want to get every single time
        if (steps.Length > 0)
        {
            stepSource.PlayOneShot(currStep.steps[Random.Range(0, currStep.steps.Length)]);
        }
    }
}
