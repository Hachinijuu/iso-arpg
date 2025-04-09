using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// For gear and tablet slots simply derive from item slots such that they can interact with the rest of the inventory system without much difficulty (slot insertion)
// With the difference, gear and tablet slots can extend the functionality to apply the functionality of the item within the slot, to the character.
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int index;
    public Image overlapImage;
    public Image image;
    //public Sprite icon;
    public GameObject selectedIndicator;
    //public TMP_Text temp;
    public ItemData item;
    public bool hovered;
    public bool HasItem { get { return item != null; } }
    public void SlotSelected(bool show)
    {
        selectedIndicator.SetActive(show);
    }

    public virtual void SetItem(ItemData data)
    {
        item = data;    // The the value of the item, null works
        if (data != null)
        {
            // Setup the image
            
            Color c = Color.white;
            c.a = 1.0f;
            image.sprite = data.itemIcon;
            image.color = c;
            if (data is RuneData rs && overlapImage != null)
            {
                overlapImage.sprite = rs.runeGlyph;
                overlapImage.color = c;
            }
        }
        else
        {
            // Data is set to null, shutoff the alpha
            Inventory.Instance.Items.Remove(data);
            Color c = Color.white;
            c.a = 0.0f;
            image.color = c;
            image.sprite = null;
            if(overlapImage != null)
            {
                overlapImage.color = c;
                overlapImage.sprite = null;
            }
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        // Having inventory slots handle an update loop would be a HORROR
        if (!hovered)
        {
            hovered = true;
            if (HasItem && item is RuneData)
            {
                ToolTipArgs args = new ToolTipArgs();
                args.over = true;
                args.screenPos = eventData.position;
                args.item = item;

                // hover works with clicking, perhaps tooltip on click
                // that would work with existing inventory framework calls

                GameplayUIController.Instance.CreateRuneTooltip(args);
            }
        }
        //throw new System.NotImplementedException();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        ToolTipArgs args = new ToolTipArgs();
        args.over = hovered;
        GameplayUIController.Instance.CreateRuneTooltip(args);
    }
}
