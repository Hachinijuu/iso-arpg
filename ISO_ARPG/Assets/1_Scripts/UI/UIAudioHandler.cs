using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioHandler : MonoBehaviour
{
    public AudioSource audioSource;
    public bool randomPitch;

    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
