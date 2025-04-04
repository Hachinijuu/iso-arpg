using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance = null;
    public static TutorialManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TutorialManager>();

            if (!instance)
                Debug.LogError("[TutorialManager]: No TutorialManager exists!");

            return instance;
        }
    }

    public AudioSource Source { get { return source; } }
    [SerializeField] AudioSource source;

    // This class will listen for gameplay events and prompt the tutorial popups accordingly
    [SerializeField] GameObject runePopup;
    [SerializeField] GameObject identityPopup;
    [SerializeField] GameObject upgradePopup;

    // Tutorial system will make use of the cutscene slides
    // Popping up the slide, and cycling the voice overs and the text accordingly


    bool runeShowed;
    bool identityShowed;
    bool upgradeShowed;

    public void NewGame()
    {
        ResetTutorial();
        AddListeners();
    }

    public delegate void FirstRune();
    public delegate void FirstHendok();
    public delegate void FirstIdentity();

    public event FirstRune OnFirstRune;
    public event FirstHendok OnFirstHendok;
    public event FirstIdentity OnFirstIdentity;

    public void FireFirstRune() { if (OnFirstRune != null) OnFirstRune(); }
    public void FireFirstHendok() { if (OnFirstHendok != null) OnFirstHendok(); }
    public void FireFirstIdentity() { if (OnFirstIdentity != null) OnFirstRune(); }

    public void AddListeners()
    {
        // Pickup rune
        // Hendok interact
        // Identity load

        if (!runeShowed) { OnFirstRune += BeginRuneTutorial; }
        if (!identityShowed) { OnFirstIdentity += BeginIdentityTutorial; }
        if (!upgradeShowed) { OnFirstHendok += BeginUpgradingTutorial; }
    }

    public void ResetTutorial()
    {
        runeShowed = false;
        identityShowed = false;
        upgradeShowed = false;
    }

    public void BeginRuneTutorial()
    {
        // Listen to the event, stop listening once it's done
        runeShowed = true;
        HandlePopup(runePopup);
        OnFirstRune -= BeginRuneTutorial;
    }

    public void BeginIdentityTutorial()
    {
        identityShowed = true;
        HandlePopup(identityPopup);
        OnFirstIdentity -= BeginIdentityTutorial;
    }
    public void BeginUpgradingTutorial()
    {
        upgradeShowed = true;
        HandlePopup(upgradePopup);
        OnFirstHendok -= BeginUpgradingTutorial;
    }

    IEnumerator HandlePopup(GameObject popup)
    {
        GameObject go = popup.gameObject;
        CutsceneSlide slide = go.GetComponent<CutsceneSlide>();
        go.SetActive(true);
        slide.EnterSlide(this);
        yield return new WaitForSeconds(slide.slideDuration);
        go.SetActive(false);
    }
}
