using TMPro;
using UnityEngine;
using UnityEngine.UI;

// For gear and tablet slots simply derive from item slots such that they can interact with the rest of the inventory system without much difficulty (slot insertion)
// With the difference, gear and tablet slots can extend the functionality to apply the functionality of the item within the slot, to the character.
public class ItemSlot : MonoBehaviour
{
    public Vector2Int index;
    public Sprite img;
    public GameObject selectedIndicator;
    //public TMP_Text temp;
    public ItemData item;
    public bool HasItem { get { return item != null; } }
    public void SlotSelected(bool show)
    {
        selectedIndicator.SetActive(show);
    }
}
