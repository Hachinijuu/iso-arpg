using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class GearSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GearSlotType type;
    public RuneSlot slot1;
    public RuneSlot slot2;

    // These rune slots will be visible, once hovered over the gear slot item, then show the combined values
    bool hovered;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        // Having inventory slots handle an update loop would be a HORROR
        if (!hovered)
        {
            hovered = true;
            if (slot1 != null && slot2 != null)
            {
                //GameplayUIController.Instance.CreateRuneTooltip(args);
                ToolTipArgs args = new ToolTipArgs();
                args.screenPos = eventData.position;
                args.gearSlot = this;
                TooltipSystem.Instance.ShowTooltip(TooltipSystem.TooltipTypes.Gear, args);
            }
        }
        //throw new System.NotImplementedException();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        ToolTipArgs args = new ToolTipArgs();
        args.over = hovered;
        TooltipSystem.Instance.HideTooltip(TooltipSystem.TooltipTypes.Rune, args);
    }
}
