using UnityEngine;

public class PlayerParticleHandler : MonoBehaviour
{
    [SerializeField] GameObject[] particles;
    [SerializeField] GameObject auraGlow;

    public void ActivateParticle(int index)
    {
        particles[index].SetActive(true);
    }

    public void DeactivateParticle(int index)
    {
        particles[index].SetActive(false);
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
