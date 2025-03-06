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


    // This will be in the persistent scene and have the source for music to play off of

    // It might also contain the references / tracks to play given the level, and functionality to play music overall (set clip)

    // Each level can have it's own track, and then set the track?
}
