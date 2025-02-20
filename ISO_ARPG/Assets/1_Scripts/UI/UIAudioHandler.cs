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
    public AudioSource audioSource;
    public bool randomPitch;

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
