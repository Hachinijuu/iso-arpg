using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutscenePlayer : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] PlayerInput input;
    public AudioSource Source { get { return source; } }
    [SerializeField] AudioSource source;

    [Header("Slide Details")]
    public List<CutsceneSlide> slides;

    [SerializeField] private float skipTime = 1.0f;
    [SerializeField] Slider skipSlider;
    public void UpdateSkipSlider()
    {
        skipSlider.value = skipCounter;
    }
    float skipCounter = 0.0f;

    #endregion

    #region BUTTON MAPPING

    #endregion
    public void Start()
    {
        StartCutscene();
    }

    #region FUNCTIONALITY
    public void StartCutscene()
    {
        if (slides != null && slides.Count > 0)
        {
            StartCoroutine(HandleCutscene());
        }
    }

    public void Update()
    {
        HandleSkip();
    }

    public void HandleSkip()
    {
        UpdateSkipSlider();
        if (Input.GetMouseButton(0))
        {
            skipCounter += Time.deltaTime;
            if (skipCounter > skipTime)
            {
                StopAllCoroutines();
                EndCutscene();
            }
        }
        else
        {
            skipCounter = 0.0f;
        }
    }
    public void StopCutscene()
    {
        StopAllCoroutines();
    }
    IEnumerator HandleCutscene()
    {
        foreach (CutsceneSlide slide in slides)
        {
            slide.gameObject.SetActive(true);
            slide.EnterSlide(this);
            yield return new WaitForSeconds(slide.slideDuration);
            slide.ExitSlide(this);
            slide.gameObject.SetActive(false);
        }
        EndCutscene();
    }

    public void EndCutscene()
    {
    GameManager.Instance.LoadLevelByID(GameManager.eLevel.HUB);
    }
    #endregion
}