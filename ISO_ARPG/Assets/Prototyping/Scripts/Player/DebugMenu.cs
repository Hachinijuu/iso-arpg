using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Playables;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerController controller;

    float ab1Timer;
    float ab2Timer;

    public void Start()
    {
        stats.Class.Abilities[0].onAbilityUsed += ListenTimer;
        stats.Class.Abilities[1].onAbilityUsed += ListenTimer;
    }

    public void ResetMana()
    {
        stats.SetMana(stats.MaxMana);
    }

    void ListenTimer(Ability used, GameObject go)
    {
        StartCoroutine(StatCountdown(used));
    }

    IEnumerator StatCountdown(Ability onCd)
    {
        bool loop = true;
        if (onCd == stats.Class.Abilities[0])
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
        else if (onCd == stats.Class.Abilities[1])
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
    private void OnGUI()
    {
        // displaying the user stats
        GUI.Box(new Rect(10, 100, 200, 360), "Player Stats");
        GUI.Label(new Rect(20, 120, 200, 20), "Health: " + stats.Health + " / " + stats.MaxHealth);
        GUI.Label(new Rect(20, 140, 200, 20), "Mana: " + stats.Mana + " / " + stats.MaxMana);
        GUI.Label(new Rect(20, 160, 200, 20), "Speed: " + stats.Speed);
        GUI.Label(new Rect(20, 180, 200, 20), "Rotation: " + stats.RotationSpeed);
        GUI.Label(new Rect(20, 200, 200, 20), "Strength: " + stats.Strength);
        GUI.Label(new Rect(20, 220, 200, 20), "Agility: " + stats.Agility);
        GUI.Label(new Rect(20, 240, 200, 20), "Wisdom: " + stats.Wisdom);

        if (GUI.Button(new Rect(20, 260, 180, 20), "Reset Mana"))
            ResetMana();
        if (GUI.Button(new Rect(20, 280, 180, 20), "Reset Ability Used"))
            controller.ResetAbilityUsage();

        GUI.Label(new Rect(20, 300, 200, 20), "Ab1 CD: " + Mathf.RoundToInt(ab1Timer));
        GUI.Label(new Rect(20, 320, 200, 20), "Ab2 CD: " + Mathf.RoundToInt(ab2Timer));
    }
}
