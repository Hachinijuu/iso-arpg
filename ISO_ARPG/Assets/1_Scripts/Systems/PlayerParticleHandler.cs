using UnityEngine;

public class PlayerParticleHandler : MonoBehaviour
{
    [SerializeField] GameObject[] ab1particles;
    [SerializeField] GameObject[] ab2particles;
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

    public void HandleAbility2Particles(bool on)
    {
        if (ab1particles != null && ab1particles.Length > 0)
        {
            foreach (GameObject particle in ab1particles)
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
