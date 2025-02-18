using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryEventArgs
{
    public ItemData data;
}

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
    Vector2 mousePos;

    private InputActionMap UI_Input;


    // Event System
    public delegate void InventoryEvent(InventoryEventArgs e);
    public event InventoryEvent OnItemReleased;
    public void FireReleasedItem(InventoryEventArgs e) { if (OnItemReleased != null) OnItemReleased(e); }

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

        int row = 0;
        int col = 0;

        foreach (ItemSlot slot in slots)
        {
            CellIndex index = new CellIndex(row, col);
            slot.index = index;

            if (col < perRow - 1)
            {
                col++;
            }
            else
            {
                col = 0;
                row++;
            }

            slot.temp.text = slot.index.x + " : " + slot.index.y;
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
        input.actions["Click"].performed += context =>
        {
            if (inventoryScreen.activeInHierarchy)
            {
                HandleItemMovement();
            }
        };    // Inside here, do the click state transitions, this will handle the dragging, where the position is only relative to the update
        input.actions["Click"].canceled += context =>
        {
            if (inventoryScreen.activeInHierarchy)
            {
                ReleaseItem();
            }
        };
    }

    public void ShowInventory()
    {
        UIUtility.ToggleUIElementShift(inventoryScreen);
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

    public void Update()
    {
        if (inventoryScreen.activeInHierarchy)
        {
            if (held)
            {
                mousePos = GetMousePos();
            }
            // ItemSlot s = GetSlotFromPos(mousePos);

            // if (s)
            // {
            //     Debug.Log(s.index.x + " : " + s.index.y);
            // }
            // Only handle logic if the inventory screen is open

            // Do movement through state transitions

            // None, Drag, Release State perhaps?
        }
    }
    Vector2 GetMousePos()
    {
        return input.actions["Point"].ReadValue<Vector2>();
    }

    ItemSlot GetSlotFromPos(Vector2 pos)
    {
        ItemSlot found = null;

        // instead of loop, try doing grid estimation relative to origin point of grid and go based off indicies, do this as a test
        foreach (ItemSlot slot in slots)
        {
            RectTransform rt = slot.GetComponent<RectTransform>();
            //Debug.Log(rt);
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

    bool CheckClickedItem()
    {
        bool clickedItem = false;
        //Vector2 click = GetMousePos();
        ItemSlot slot = GetSlotFromPos(mousePos);

        if (slot != null)
        {
            if (slot.HasItem)
            {
                sourceSlot = slot;
                ghostItem = sourceSlot.item;
                Debug.Log("[Inventory]: Source Slot: " + "(" + sourceSlot.index.x + " : " + sourceSlot.index.y + ")");

                // release the image when clicked, and use the image as a ghost

                // 'pickup the item' --> set the values to null, and have the init slot that follows the mouse be the item.
                StartCoroutine(HandleGhostItem());
                slot.img.sprite = null;
                slot.item = null;

                //ghost = GameObject.Instantiate(sourceSlot.gameObject, canvas.transform);
                clickedItem = true;
            }
        }
        return clickedItem;
    }

    public void HandleItemMovement()
    {
        mousePos = GetMousePos();
        //Debug.Log(mousePos);
        if (CheckClickedItem())
        {
            // If an item has been clicked, we want to account for it's release on a valid slot
            Debug.Log("Item is held");
            held = true;

            // Create a ghost here while the item movement exists
        }
    }

    IEnumerator HandleGhostItem()
    {
        ghost = GameObject.Instantiate(sourceSlot.gameObject, canvas.transform); // probably don't instantiate, just active existing ghost set to mouse position
        Image gImage = ghost.GetComponent<Image>();
        gImage.sprite = sourceSlot.img.sprite;

        RectTransform rt = ghost.GetComponent<RectTransform>();
        rt.sizeDelta = sourceSlot.GetComponent<RectTransform>().sizeDelta;
        //rt.localScale = new Vector3(100,100);
        do
        {
            if (rt != null)
                rt.position = mousePos;
            yield return new WaitForEndOfFrame();
        }
        while (held);
        Destroy(ghost);
    }

    void ReleaseItem()
    {

        if (ghostItem)
        {
            // This is fired whenever the mouse is released
            ItemSlot slot = GetSlotFromPos(mousePos);

            if (slot != null)
            {
                if (slot == sourceSlot || slot.HasItem) // Click and release on the same position
                {
                    Debug.Log("[Inventory]: Failed placement, reverting");
                    // Revert item back to slot
                    sourceSlot.item = ghostItem;
                    ghostItem.LoadSpriteToImage(sourceSlot.img);
                }
                else
                {
                    Debug.Log("[Inventory]: Index (" + slot.index.x + " : " + slot.index.y + ") is empty, placing item here");
                    slot.item = ghostItem;
                    ghostItem.LoadSpriteToImage(slot.img);
                    // in this, check if the slot IS A rune slot, if it is a rune slot, if it is a rune slot, we can only place runes in said slots
                    //if (slot is RuneSlot rs && ghostItem.type == ItemTypes.RUNE)
                    //{
                    //    if (ghostItem.type == ItemTypes.RUNE)
                    //    {
                    //        Debug.Log("[Inventory]: Embedding rune");
                    //        slot.item = ghostItem;
                    //        ghostItem.LoadSpriteToImage(slot.img);
                    //    }
                    //}
                    //else
                    //{
                    //    Debug.Log("[Inventory]: Index (" + slot.index.x + " : " + slot.index.y + ") is empty, placing item here");
                    //    slot.item = ghostItem;
                    //    ghostItem.LoadSpriteToImage(slot.img);
                    //}
                }

                //Debug.Log("[Inventory]: Mouse released at: (" + slot.index.x + " : " + slot.index.y + ")" );
                // if (!slot.HasItem)
                // {
                //     Debug.Log("[Inventory]: Index (" + slot.index.x + " : " + slot.index.y + ") is empty, placing item here");
                //     slot.item = ghostItem;
                //     ghostItem.LoadSpriteToImage(slot.img);
                // }
                // else
                // {
                //     Debug.Log("[Inventory]: Index (" + slot.index.x + " : " + slot.index.y + ") is occupied, do not place item here");
                //     // Revert item back to source slot
                //     sourceSlot.item = ghostItem;
                //     ghostItem.LoadSpriteToImage(sourceSlot.img);
                // }
                // Regardless, set the ghost to null
                ghostItem = null;
            }
            else
            {
                Debug.Log("[Inventory]: Did not release mouse on a valid position");
                Debug.Log("[Inventory]: Failed placement, reverting");
                // Revert item back to slot

                // If release position is outside of the canvas --> we can detect to drop the item / discard
                sourceSlot.item = ghostItem;
                ghostItem.LoadSpriteToImage(sourceSlot.img);
            }

            InventoryEventArgs args = new InventoryEventArgs();
            args.data = slot.item;
            FireReleasedItem(args);
        }


        // NOTE: ISSUE WITH GHOST PLACEMENTS, STUCK IN CURSOR ON INVALID POSITIONS
        if (ghost)
        {
            Destroy(ghost);
        }

        // Vector2 pos = GetMousePos();
        // ItemSlot s = GetSlotFromPos(pos);
        // if (s)
        //     Debug.Log("Slot exists");

        // if (held == false && ghostItem != null)
        // {
        //     ItemSlot slot = GetSlotFromPos(pos);

        //     // Destroy the ghost even if it has not been placed in the valid slot (removal)
        //     //Destroy(ghost); 

        //     if (slot)
        //     {
        //         Debug.Log("Is there an item?: " + slot.HasItem);
        //         if (!slot.HasItem)
        //         {
        //             slot.item = ghostItem;
        //             ghostItem.LoadSpriteToImage(slot.img);
        //             sourceSlot.item = null;
        //             sourceSlot.img.sprite = null;
        //             ghostItem = null;
        //         }
        //         else
        //             Debug.Log("[Inventory]: Cannot move item, item already exists in that slot");
        //     }
        // }
    }
}
