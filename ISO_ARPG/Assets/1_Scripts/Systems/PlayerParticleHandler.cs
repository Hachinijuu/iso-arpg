using UnityEngine;
using System.Collections;

public class PlayerParticleHandler : MonoBehaviour
{
    [SerializeField] public GameObject[] ab1particles;
    [SerializeField] public GameObject[] ab2particles;
    [SerializeField] GameObject auraGlow;

    public void HandleAbility1Particles(bool on)
    {
        if (ab1particles != null && ab1particles.Length > 0)
        {
            foreach (GameObject particle in ab1particles)
            {
                particle.SetActive(on);
            }
        }
    }

    public void StartParticlesFor(float timeFor)
    { 
        StartCoroutine(PlayParticlesFor(timeFor));
    }
    
    public IEnumerator PlayParticlesFor(float timeFor)
    {
        HandleAbility1Particles(true);
        yield return new WaitForSeconds(timeFor);
        HandleAbility2Particles(false);
    }

    public void HandleAbility2Particles(bool on)
    {
        if (ab2particles != null && ab2particles.Length > 0)
        {
            foreach (GameObject particle in ab2particles)
            {
                particle.SetActive(on);
            }
        }
    }

    public void ActivateAura()
    {
        auraGlow.SetActive(true);
    }

    public void DeactivateAura()
    {
        auraGlow.SetActive(false);
    }

}
