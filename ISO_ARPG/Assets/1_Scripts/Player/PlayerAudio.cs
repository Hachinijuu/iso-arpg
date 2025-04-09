using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // This class will be responsible for playing sounds on the player

    // Mainly voices
    // Cancel / Unable sounds for general elements (potion usage)

    [Header("Sources")]
    [SerializeField] AudioSource voiceSource;   // The source this audio will play from (VOICES MAINLY)
    [SerializeField] AudioSource effectSource;  // This source will play audio for effects (cannot use potion)

    // Effects, footsteps etc, will play from an alternate audio source, that way mixing can be done separately

    // Have SO containers for sources, and adjust the sounds in them accordingly

    [Header("Sounds")]
    [SerializeField] SoundHolder efforts;
    [SerializeField] SoundHolder hurts;
    [SerializeField] AudioClip cancelSound;
    [SerializeField] AudioClip potionUse;

    [Header("Audio Settings")]
    public float minPitch = 0.0f;
    public float maxPitch = 1.0f;
    public void PlayEffort()
    { 
        // Cycle between the efforts and play a sound
        if (efforts.sounds.Length <= 0 && efforts.sounds == null) { return; }
        int index = Random.Range(0, efforts.sounds.Length);
        voiceSource.clip = efforts.sounds[index];

        // do pitch adjustment for variable sounds
        voiceSource.pitch = Random.Range(minPitch, maxPitch);
        voiceSource.Play();
    }

    public void PlayHurt()
    {
        // Cycle between hurt sounds and play a hurt sound
        if (hurts.sounds.Length <= 0 && hurts.sounds == null) { return; }
        int index = Random.Range(0, hurts.sounds.Length);
        voiceSource.clip = hurts.sounds[index];

        // do pitch adjustment for variable sounds
        voiceSource.pitch = Random.Range(minPitch, maxPitch);
        voiceSource.Play();
    }

    public void PlayDeath()
    { 
        // Pending death sound
    }

    public void PlayUnable()
    {
        // Unable sound
        effectSource.PlayOneShot(cancelSound);
    }

    public void PlayPotionUse()
    {
        effectSource.PlayOneShot(potionUse);
    }
}
