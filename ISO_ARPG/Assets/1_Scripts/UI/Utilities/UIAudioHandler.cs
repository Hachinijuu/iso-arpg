using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioHandler : MonoBehaviour
{
    private static UIAudioHandler instance = null;
    public static UIAudioHandler Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UIAudioHandler>();

            if (!instance)
                Debug.LogError("[UIAudioHandler]: No handler exists!");

            return instance;
        }
    }

    public void Start()
    {
        if (PublicEventManager.Instance != null) { PublicEventManager.Instance.onCannot += OnCannot; }
    }
    public AudioSource audioSource;
    public bool randomPitch;

    public void PlayAudio(AudioClip clip)
    {
        if (clip == null) { return; }
        audioSource.PlayOneShot(clip);
    }

    // These effects are hooked up the SFX audio source
    // This will play unable sounds

    [SerializeField] AudioClip cannotClip;

    public void OnCannot()
    {
        if (audioSource != null && cannotClip != null)
        {
            PlayAudio(cannotClip);
        }
    }
}
