using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private static Inventory instance = null;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Inventory>();

            if (!instance)
                Debug.LogError("[Inventory]: No inventory exists!");

            return instance;
        }
    }
    [SerializeField] HUDController hud;
    [SerializeField] GameObject inventoryScreen;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject slotHolder;
    [SerializeField] int perRow;

    int maxSlots;

    [SerializeField] PlayerInput input;

    private ItemSlot[] slots;       // UI slots to interface with
    private List<ItemData> items;   // The actual items the player has

    // input handling
    bool held = false;
    private ItemSlot sourceSlot;
    private GameObject ghost;
    private ItemData ghostItem;

    private InputActionMap UI_Input;

    public void Start()
    {
        InitUISlots();
        InitItemList();
        MapActions();
    }

    void InitUISlots()
    {
        slots = slotHolder.GetComponentsInChildren<ItemSlot>();

        if (slots != null && slots.Length > 0)
        {
            maxSlots = slots.Length;
            // int row = 0;
            // int col = 0;

            // foreach (ItemSlot slot in slots)
            // {

            // }

            // does the index of the slot even matter?
            // at this point no.. for faster lookup, maybe, but that requires external list of indices, rather than the slot itself holding the index (redundant)
        }
    }

    void InitItemList()
    {
        items = new List<ItemData>();
    }

    void MapActions()
    {
        //UI_Input = InputActionMap("UI");
        // based on the action map, we want to map inventory open to I
        // want to map UI click to handling inventory movement
        input.actions["OpenInventory"].performed += context => { ShowInventory(); };
        input.actions["Click"].performed += context => {held = CheckClickedItem(ref ghostItem); ReleaseItem(); };
    }

    public void ShowInventory()
    {
        hud.ToggleUIElementShift(inventoryScreen);
        //if (!inventoryScreen.activeInHierarchy)
        //{
        //    //input.SwitchCurrentActionMap("UI");
        //}
        //input.SwitchCurrentActionMap("Player");

        // swap action map to UI -- temporary

    }
    public void AddItem(ItemData data)
    {
        ItemSlot slotToUse = FindFreeSlot();
        // ONLY ADD TO THE SLOTS IF THERE IS ENOUGH SPACE AS WELL
        if (items.Count <= maxSlots)
        {
            if (slotToUse != null)
            {
                slotToUse.item = data;
                //Image img = slotToUse.GetComponent<Image>();
                //if (img != null)
                data.LoadSpriteToImage(slotToUse.img);
                items.Add(data);
            }
        }
    }

    public ItemSlot FindFreeSlot()
    {
        foreach (ItemSlot slot in slots)
        {
            if (!slot.HasItem)
            {   
                return slot;
            }
        }
        return null;
    }

    Vector2 GetMousePos()
    {
        return input.actions["Point"].ReadValue<Vector2>();
    }

    ItemSlot GetSlotFromPos(Vector2 pos)
    {
        ItemSlot found = null;
        foreach (ItemSlot slot in slots)
        {
            RectTransform rt = slot.GetComponent<RectTransform>();
            if (rt)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, pos))
                {
                    found = slot;
                    break;
                }
            }
        }
        return found;
    }

    bool CheckClickedItem(ref ItemData heldItem)
    {
        bool clickedItem = false;
        Vector2 click = GetMousePos();
        ItemSlot slot = GetSlotFromPos(click);

        if (slot != null)
        {   
            if (slot.HasItem)
            {
                sourceSlot = slot;
                ghostItem = sourceSlot.item;

                //ghost = GameObject.Instantiate(sourceSlot.gameObject, canvas.transform);
                clickedItem = true;
            }
        }
        return clickedItem;
    }

    void ReleaseItem()
    {
        if (held == false && ghostItem != null)
        {
            Vector2 pos = GetMousePos();
            ItemSlot slot = GetSlotFromPos(pos);
            
            // Destroy the ghost even if it has not been placed in the valid slot (removal)
            //Destroy(ghost); 
            
            if (slot)
            {
                if (!slot.HasItem)
                {
                    slot.item = ghostItem;
                    ghostItem.LoadSpriteToImage(slot.img);
                    sourceSlot.item = null;
                    sourceSlot.img.sprite = null;
                    ghostItem = null;
                }
                else
                    Debug.Log("[Inventory]: Cannot move item, item already exists in that slot");
            }
        }
    }
}
