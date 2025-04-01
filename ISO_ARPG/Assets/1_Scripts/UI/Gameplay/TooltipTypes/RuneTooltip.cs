using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuneTooltip : MonoBehaviour
{
    [SerializeField] TMP_Text rarityText;
    [SerializeField] TMP_Text attributeAmount;
    [SerializeField] TMP_Text affixAmount;

    public void SetRuneData(RuneData rune)
    {
        rarityText.text = rune.rarity.ToString();
        attributeAmount.text = rune.mainStat[0].Value.ToString();
        if (rune.subStat == null || rune.trackedStat == null) { return; }

        if (rune.subStat.Length > 0)
        {
            affixAmount.text = rune.subStat[0].Value.ToString();
        }
        else if (rune.trackedStat.Length > 0)
        {
            affixAmount.text = rune.trackedStat[0].Value.ToString();
        }
    }
}
