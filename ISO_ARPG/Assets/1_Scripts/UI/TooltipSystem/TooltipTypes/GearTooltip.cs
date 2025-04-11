using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GearTooltip : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text main;
    public TMP_Text amount;
    public TMP_Text affix1;
    public TMP_Text affix2;
    public TMP_Text amount1;
    public TMP_Text amount2;

    public void LoadFromGear(GearSlot slot)
    {
        itemName.text = slot.type.ToString();
        RuneData rune1 = slot.slot1.item as RuneData;
        RuneData rune2 = slot.slot2.item as RuneData;

        int attributeTotal = 0;

        if (rune1 != null)
        {
            attributeTotal += (int)rune1.mainStat[0].Value;
            if (rune1.subStat == null || rune1.trackedStat == null) { return; }
            if (rune1.subStat.Length > 0)
            {
                affix1.text = rune1.subStat[0].type.ToString();
                amount1.text = rune1.subStat[0].Value.ToString();
            }
            else if (rune1.trackedStat.Length > 0)
            {
                affix1.text = rune1.trackedStat[0].type.ToString();
                amount1.text = rune1.trackedStat[0].Value.ToString();
            }
        }
        else
        {
            affix1.text = "NONE";
            amount1.text = "0";
        }
        
        if (rune2 != null)
        {
            attributeTotal += (int)rune2.mainStat[0].Value;
            if (rune2.subStat == null || rune2.trackedStat == null) { return; }
            if (rune2.subStat.Length > 0)
            {
                affix2.text = rune2.subStat[0].type.ToString();
                amount2.text = rune2.subStat[0].Value.ToString();
            }
            else if (rune2.trackedStat.Length > 0)
            {
                affix2.text = rune2.trackedStat[0].type.ToString();
                amount2.text = rune2.trackedStat[0].Value.ToString();
            }
        }
        else
        {
            affix2.text = "NONE";
            amount2.text = "0";
        }

        if (rune1 != null || rune2 != null)
        {
            if (rune1 != null)
            {
                main.text = rune1.mainStat[0].type.ToString();
            }
            else if (rune2 != null)
            {
                main.text = rune2.mainStat[0].type.ToString();
            }
            amount.text = attributeTotal.ToString();
        }
        else
        {
            main.text = "NONE";
            amount.text = attributeTotal.ToString();
        }
    }


    // public void SetRuneData(RuneData rune)
    // {
    //     rarityText.text = rune.rarity.ToString();
    //     rarityBG.color = TooltipSystem.Instance.GetColourFromRarity(rune.rarity);
    //     attributeText.text = rune.mainStat[0].type.ToString();
    //     attributeAmount.text = rune.mainStat[0].Value.ToString();
    //     if (rune.subStat == null || rune.trackedStat == null) { return; }

    //     if (rune.subStat.Length > 0)
    //     {
    //         affixText.text = rune.subStat[0].type.ToString();
    //         affixAmount.text = rune.subStat[0].Value.ToString();
    //     }
    //     else if (rune.trackedStat.Length > 0)
    //     {
    //         string updatedAffix = rune.trackedStat[0].type.ToString();
    //         string formatted = updatedAffix.Replace("_", " ");
    //         affixText.text = formatted;
    //         affixAmount.text = rune.trackedStat[0].Value.ToString();
    //     }
    // }
}
