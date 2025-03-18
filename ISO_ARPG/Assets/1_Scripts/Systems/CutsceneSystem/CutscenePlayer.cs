using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutscenePlayer : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] PlayerInput input;
    public AudioSource Source { get { return source; } }
    [SerializeField] AudioSource source;

    [Header("Slide Details")]
    public List<CutsceneSlide> slides;

    [SerializeField] private float skipTime;
    [SerializeField] private float skipInterval;

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
        GameManager.Instance.LoadLevelByID(GameManager.eLevel.HUB);
    }
    #endregion
}