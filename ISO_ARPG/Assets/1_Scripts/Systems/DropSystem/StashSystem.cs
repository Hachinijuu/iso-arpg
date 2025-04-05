using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashSystem : MonoBehaviour
{
    [SerializeField] GameObject slotHolder;
    public ItemSlot[] slots;
    public void Start()
    {
        GetSlots();
    }
    public void GetSlots()
    {
        slots = slotHolder.GetComponentsInChildren<ItemSlot>();
    }
    public void CleanupStash()
    {
        if (slots == null && slots.Length <= 0) { return; }
        foreach (ItemSlot slot in slots)
        {
            if (slot.HasItem)
            {
                slot.SetItem(null);
            }
        }
    }
}
