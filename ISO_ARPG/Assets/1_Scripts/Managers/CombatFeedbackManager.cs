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

    public GameObject[] hitParticles;    
    public GameObject[] chainParticles;
    public GameObject[] critParticles;

    public AudioClip[] hitSounds;


    public AudioSource audioSource;

    public const int MAX_HITSOUNDS = 5;
    public const int MAX_HITPARTICLES = 5;

    // play the one shots off of the source, and then only allow more sounds if the counter is 0
    public void PlayHitSounds()
    {
    }

    public void PlayHitParticles(Vector3 particlePos)
    {
            int randomParticle = Random.Range(0, hitParticles.Length);

            GameObject particles = GameObject.Instantiate(hitParticles[randomParticle]);
            particles.transform.position = particlePos;
    }

    public void PlayCritParticles(Vector3 particlePos)
    {
        int randomParticle = Random.Range(0, critParticles.Length);

        GameObject particles = GameObject.Instantiate(critParticles[randomParticle]);
        particles.transform.position = particlePos;
    }

    public void PlayChainParticles(Vector3 particlePos)
    {
        int randomParticle = Random.Range(0, chainParticles.Length);

        GameObject particles = GameObject.Instantiate(chainParticles[randomParticle]);
        particles.transform.position = particlePos;
    }
}
