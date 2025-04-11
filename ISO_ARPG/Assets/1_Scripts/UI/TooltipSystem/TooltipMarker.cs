using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ItemData item;
    Ability ability;
    TooltipSystem.TooltipTypes type;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        ToolTipArgs args = new ToolTipArgs();
        if (item != null) { args.item = item; }
        if (ability != null) {args.ability = ability; }
        TooltipSystem.Instance.ShowTooltip(type, args);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.HideTooltip(type, new ToolTipArgs());
    }
}
