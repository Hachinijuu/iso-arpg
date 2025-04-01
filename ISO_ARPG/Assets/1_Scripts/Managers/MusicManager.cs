using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance = null;
    public static MusicManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MusicManager>();

            if (!instance)
                Debug.LogError("[MusicManager]: No Music Manager exists!");

            return instance;
        }
    }
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }


    [SerializeField] AudioClip titleMusic;
    [SerializeField] AudioClip hubMusic;
    [SerializeField] AudioClip battleMusic;

    // This will be in the persistent scene and have the source for music to play off of

    // It might also contain the references / tracks to play given the level, and functionality to play music overall (set clip)

    // Each level can have it's own track, and then set the track?

    public void SetTitleMusic()
    { 
        if (titleMusic == null) { return; }
        audioSource.clip = titleMusic;
        audioSource.Play();
    }

    public void SetHubMusic()
    {
        if (hubMusic == null) { return; }
        audioSource.clip = hubMusic;
        audioSource.Play();
    }

    public void SetBattleMusic()
    {
        if (battleMusic == null) { return; }
        audioSource.clip = battleMusic;
        audioSource.Play();
    }
}
