using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerController controller;

    //private static string targetTextStatic = "Target: ";
    private string hoverText;
    private string targetText;

    float ab1Timer;
    float ab2Timer;

    public void Start()
    {
        // Listen for onAbilityEnded for the countdowns, because for channeled abilities, the ability will not go on cooldown until channeling has been completed
        // This may be changed later on, but events can be called accordingly.
        // NOTE: For UI displays, listening for abilityUsed / abilityEnded might cause issues with timer displays
        // Players can continously press the buttons even though the timer has not yet completed, causing the timer to reset its display
        // This can be remedied by the AbilityHandler sending information to the UI of it's internal cooldown count. (most likely solution)

        stats.Class.abilities[0].onAbilityEnded += ListenTimer;
        stats.Class.abilities[1].onAbilityEnded += ListenTimer;
    }

    public void ResetMana()
    {
        stats.Mana.SetValue(stats.Mana.MaxValue);
    }

    public void FillIDBar()
    { 
        stats.ID_Bar.SetValue(stats.ID_Bar.MaxValue);
    }

    void ListenTimer(AbilityEventArgs e)
    {
        StartCoroutine(StatCountdown(e.Ability));
    }

    IEnumerator StatCountdown(Ability onCd)
    {
        bool loop = true;
        if (onCd == stats.Class.abilities[0])
        {
            ab1Timer = onCd.Cooldown;
            while (loop)
            {
                ab1Timer -= Time.deltaTime;
                if (ab1Timer <= 0)
                    loop = false;
                yield return null;
            }
        }
        else if (onCd == stats.Class.abilities[1])
        {
            ab2Timer = onCd.Cooldown;
            while (loop)
            {
                ab2Timer -= Time.deltaTime;
                if (ab2Timer <= 0)
                    loop = false;
                yield return null;
            }
        }
    }

    public void UpdateTargetText(string value)
    {
        targetText = "Target: " + value;
    }

    public void UpdateHoverText(string value)
    {
        hoverText = "Hovering: " + value;
    }
    private void OnGUI()
    {
        // displaying the user stats
        GUI.Box(new Rect(10, 100, 200, 360), "Player Stats");
        GUI.Label(new Rect(20, 120, 200, 20), "Health: " + stats.Health.Value + " / " + stats.Health.MaxValue);
        GUI.Label(new Rect(20, 140, 200, 20), "Mana: " + stats.Mana.Value + " / " + stats.Mana.MaxValue);
        GUI.Label(new Rect(20, 160, 200, 20), "ID: " + stats.ID_Bar.Value + " / " + stats.ID_Bar.MaxValue);
        //GUI.Label(new Rect(20, 160, 200, 20), "Mana: " + stats.Mana.Value + " / " + stats.Mana.MaxValue);
        //GUI.Label(new Rect(20, 180, 200, 20), "Rotation: " + stats.RotationSpeed);
        GUI.Label(new Rect(20, 200, 200, 20), "Strength: " + stats.STR.Value);
        GUI.Label(new Rect(20, 220, 200, 20), "Dexterity: " + stats.DEX.Value);
        GUI.Label(new Rect(20, 240, 200, 20), "Intelligence: " + stats.INT.Value);

        if (GUI.Button(new Rect(20, 260, 180, 20), "Reset Mana"))
            ResetMana();
        if (GUI.Button(new Rect(20, 280, 180, 20), "Reset Ability Used"))
            controller.ResetAbilityUsage();
        if (GUI.Button(new Rect(20, 300, 180, 20), "Fill ID"))
            FillIDBar();


        GUI.Label(new Rect(20, 320, 200, 20), "Ab1 CD: " + Mathf.RoundToInt(ab1Timer));
        GUI.Label(new Rect(20, 340, 200, 20), "Ab2 CD: " + Mathf.RoundToInt(ab2Timer));

        GUI.Label(new Rect(20, 360, 200, 20), hoverText);
        GUI.Label(new Rect(20, 380, 200, 20), targetText);
    }
}
