using UnityEngine;
using UnityEngine.UI;

// For gear and tablet slots simply derive from item slots such that they can interact with the rest of the inventory system without much difficulty (slot insertion)
// With the difference, gear and tablet slots can extend the functionality to apply the functionality of the item within the slot, to the character.
public class ItemSlot : MonoBehaviour
{
    public Vector2Int index;
    public Image image;
    //public Sprite icon;
    public GameObject selectedIndicator;
    //public TMP_Text temp;
    public ItemData item;
    public bool HasItem { get { return item != null; } }
    public void SlotSelected(bool show)
    {
        selectedIndicator.SetActive(show);
    }

    public void SetItem(ItemData data)
    {
        item = data;    // The the value of the item, null works
        if (data != null)
        {
            // Setup the image
            Color c = Color.white;
            c.a = 1.0f;
            image.sprite = data.itemIcon;
            image.color = c;
        }
        else
        {
            // Data is set to null, shutoff the alpha
            Color c = Color.white;
            c.a = 0.0f;
            image.color = c;
            image.sprite = null;
        }
    }


    // SEE MOUSE OVER EVENT
    private void OnMouseOver()
    {
        // THIS MAY BE THE PROPER FUNCTION FOR UI HOVER DETECTION
    }

    // THIS IS ONLY IF COLLIDER EXISTS, UI SLOTS NEED DIFFERENT SETUP
    protected void OnMouseEnter()
    {
        GameplayUIController.Instance.FireMouseHovered(new MouseHoverEventArgs(gameObject));
    }

    protected void OnMouseExit()
    {
        GameplayUIController.Instance.FireMouseExit(new MouseHoverEventArgs(null));
    }
}
