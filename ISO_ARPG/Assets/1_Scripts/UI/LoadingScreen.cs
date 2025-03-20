using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour 
{
    // empty class for later
    [SerializeField] Slider progressBar;
    [SerializeField] CanvasGroup cg;
    float fadeDuration = 0.5f;
    public void FadeIn()
    {
        cg.alpha = 0.0f;
        StartCoroutine(HandleFadeIn(fadeDuration));
    }

    public void FadeOut()
    {
        cg.alpha = 1.0f;
        StartCoroutine(HandleFadeOut(fadeDuration));
    }

    IEnumerator HandleFadeOut(float fadeTime)
    {
        while (cg.alpha > 0.1f)
        {
            cg.alpha += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0.0f;
    }
    IEnumerator HandleFadeIn(float fadeTime)
    {
        while (cg.alpha < 0.9f)
        {
            cg.alpha += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1.0f;
    }
    public void UpdateSlider(float value)
    {
        //progressBar.value = value;
    }
}