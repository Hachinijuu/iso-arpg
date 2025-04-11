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
    
    public ObjectPool[] hitParticlePool;
    public ObjectPool[] chainParticlePool;
    public ObjectPool[] critParticlePool;
    // public GameObject[] hitParticles;    
    // public GameObject[] chainParticles;
    // public GameObject[] critParticles;
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
        if (hitParticlePool.Length <= 0 || hitParticlePool == null) { return; }
        int randomParticle = Random.Range(0, hitParticlePool.Length);
        GameObject particle = hitParticlePool[randomParticle].GetPooledObject();
        if (particle == null) { return; }
        particle.transform.position = particlePos; 
        
        StartCoroutine(HandleParticle(particle));
            // GameObject particles = GameObject.Instantiate(hitParticles[randomParticle]);
            // particles.transform.position = particlePos;
    }

    public float particleDuration = 0.25f;
    public IEnumerator HandleParticle(GameObject particle)
    {
        particle.SetActive(true);
        yield return new WaitForSeconds(particleDuration);
        particle.SetActive(false);
    }



    public void PlayCritParticles(Vector3 particlePos)
    {
        if (critParticlePool.Length <= 0 || critParticlePool == null) { return; }
        int randomParticle = Random.Range(0, critParticlePool.Length);
        GameObject particle = critParticlePool[randomParticle].GetPooledObject();
        if (particle == null) { return; }
        particle.transform.position = particlePos;
        StartCoroutine(HandleParticle(particle));

    }

    public void PlayChainParticles(Vector3 particlePos)
    {
        if (chainParticlePool.Length <= 0 || chainParticlePool == null) { return; }
        Debug.Log("I built chain particles");
        int randomParticle = Random.Range(0, chainParticlePool.Length);
        GameObject particle = chainParticlePool[randomParticle].GetPooledObject();
        if (particle == null) { return; }
        particle.transform.position = particlePos;
        StartCoroutine(HandleParticle(particle));

    }
}
