using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatFeedbackManager : MonoBehaviour
{
    private static CombatFeedbackManager instance = null;
    public static CombatFeedbackManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CombatFeedbackManager>();
            if (!instance)
                Debug.LogWarning("[CombatFeedbackManager]: No AI manager found");
            return instance;
        }
    }

    GameObject[] hitParticles;
    GameObject[] chainParticles;
    AudioClip[] hitSounds;


    public AudioSource audioSource;

    public const int MAX_HITSOUNDS = 5;
    public const int MAX_HITPARTICLES = 5;

    // play the one shots off of the source, and then only allow more sounds if the counter is 0
    public void PlayHitSounds()
    {
    }

    public void PlayHitParticles()
    { 
        
    }
}
