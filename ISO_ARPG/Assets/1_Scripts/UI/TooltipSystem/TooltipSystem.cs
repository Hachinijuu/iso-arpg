using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TooltipSystem : MonoBehaviour
{
    public enum TooltipTypes { Rune, Gear, Ability }
    public GameObject runeTooltip;
    public GameObject gearTooltip;
    public GameObject abilityTooltip;
    public Canvas tooltipCanvas;
    GameObject tooltip;
    private static TooltipSystem instance = null;
    public static TooltipSystem Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TooltipSystem>();

            if (!instance)
                Debug.LogError("[Inventory]: No Tooltip System exists!");

            return instance;
        }
    }    

    public void ShowTooltip(TooltipTypes type, ToolTipArgs e)
    {
        switch (type)
        {
            case TooltipTypes.Rune:
                CreateRuneTooltip(e);
            break;
            case TooltipTypes.Gear:
                CreateGearTooltip();
            break;
            case TooltipTypes.Ability:
            break;
        }
    }
    public void HideTooltip(TooltipTypes type, ToolTipArgs e)
    {
        if (tooltip != null)
        {
            Destroy(tooltip);
        }
    }
    public void CreateRuneTooltip(ToolTipArgs e)
    {
        // Get the information from the hovered element
        //if (e.over) { return; }
        tooltip = Instantiate(runeTooltip, tooltipCanvas.transform);
        RuneTooltip info = tooltip.GetComponent<RuneTooltip>();
        RectTransform rt = tooltip.GetComponent<RectTransform>();

        info.SetRuneData(e.item as RuneData);
        Vector2 tooltipPos = e.screenPos;


        if (rt != null)
        {
            float tooltipWidth = rt.rect.width;
            float tooltipHeight = rt.rect.height;
            // x bounds checks
            if (tooltipPos.x + rt.rect.width > Screen.width)
            {
                tooltipPos.x = Screen.width - tooltipWidth;
            }
            if (tooltipPos.x < 0)
            {
                tooltipPos.x = 0;
            }
            // y bounds checks
            if (tooltipPos.y - tooltipHeight < 0)
            {
                tooltipPos.y = tooltipWidth;
            }
            if (tooltipPos.y + tooltipHeight > Screen.height)
            {
                tooltipPos.y = Screen.height - tooltipHeight;
            }
            rt.position = tooltipPos;

            
        }
    }

    public void CreateGearTooltip()
    {
        // Get the information from the hovered element

        tooltip = Instantiate(gearTooltip, tooltipCanvas.transform);
        RectTransform rt = tooltip.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.position = Input.mousePosition;
        }
    }

    public void CreateAbilityTooltip()
    {
        tooltip = Instantiate(abilityTooltip, tooltipCanvas.transform);
        RectTransform rt = tooltip.GetComponent<RectTransform>();

        if (rt != null)
        {
            rt.position = Input.mousePosition;
        } 
    }
    // IEnumerator HandleRuneTooltip(ToolTipArgs e)
    // {
    //     tooltip = Instantiate(runeTooltip, tooltipCanvas.transform);   // Parent it to the inventory
    //     RectTransform rect = tooltip.GetComponent<RectTransform>();
    //     RuneTooltip rt = tooltip.GetComponent<RuneTooltip>();       
        
    //     if (!(e.item is RuneData)) { yield break; } // If the item is not rune data, eject
    //     if (rt != null)
    //     {
    //         rect.position = e.screenPos;
    //         rt.SetRuneData(e.item as RuneData);
    //     }
    //     do
    //     {
    //         yield return new WaitForEndOfFrame();
    //     } while (tipActive);
    //     Destroy(tooltip);
    // }

    public void CleanupTooltips()
    {
        RuneTooltip[] tooltips = tooltipCanvas.GetComponentsInChildren<RuneTooltip>();
        if (tooltips == null && tooltips.Length <= 0) { return; }
        foreach (RuneTooltip tooltip in tooltips)
        {
            Destroy(tooltip.gameObject);
        }
    }
}

public class ToolTipArgs
{
    public bool over;
    public ItemData item;
    public Vector2 screenPos;
    public Ability ability;
}