using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuneTooltip : MonoBehaviour
{
    [SerializeField] Image rarityBG; 
    [SerializeField] TMP_Text rarityText;
    [SerializeField] TMP_Text attributeText;
    [SerializeField] TMP_Text affixText;
    [SerializeField] TMP_Text attributeAmount;
    [SerializeField] TMP_Text affixAmount;

    public void SetRuneData(RuneData rune)
    {
        rarityText.text = rune.rarity.ToString();
        rarityBG.color = TooltipSystem.Instance.GetColourFromRarity(rune.rarity);
        attributeText.text = rune.mainStat[0].type.ToString();
        attributeAmount.text = rune.mainStat[0].Value.ToString();
        if (rune.subStat == null || rune.trackedStat == null) { return; }

        if (rune.subStat.Length > 0)
        {
            affixText.text = rune.subStat[0].type.ToString();
            affixAmount.text = rune.subStat[0].Value.ToString();
        }
        else if (rune.trackedStat.Length > 0)
        {
            string updatedAffix = rune.trackedStat[0].type.ToString();
            string formatted = updatedAffix.Replace("_", " ");
            affixText.text = formatted;
            affixAmount.text = rune.trackedStat[0].Value.ToString();
        }
    }
}
