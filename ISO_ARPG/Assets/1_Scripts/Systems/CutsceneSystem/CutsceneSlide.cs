using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CutsceneSlide : MonoBehaviour 
{
    // Cutscene slide handles itself with timings between each step (subtitles)
    // IMAGES do not cycle on a given slide, ONE slide has a SINGLE image, and the subtitle transitions accordingly
    #region VARIABLES
    [Header("Slide Details")]
    public Image slideGraphic;
    public TMP_Text slideText;
    public AudioClip slideVoiceover;
    public float slideDuration;

    [Header("Subtitle Details")]
    public float subtitleTime;
    public List<string> subtitleText;
    #endregion

    #region FUNCTIONALITY
    public void EnterSlide(CutscenePlayer player)
    {
        player.Source.clip = slideVoiceover;
        player.Source.Play();

        StartCoroutine(HandleSubtitles(subtitleTime));
    }

    public void ExitSlide(CutscenePlayer player)
    {
        // Can do stuff here, play transition noise, fade nicely, pause audio if once clip, test.
    }

    IEnumerator HandleSubtitles(float timing)
    {
        foreach (string subtitle in subtitleText)
        {
            slideText.text = subtitle;
            yield return new WaitForSeconds(timing);
        }
    }
    #endregion
}