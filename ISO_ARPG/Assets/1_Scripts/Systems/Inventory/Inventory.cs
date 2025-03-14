using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] TMP_Text dustText;
    [SerializeField] TMP_Text woodText;
    [SerializeField] TMP_Text stoneText;

    int maxSlots;

    [SerializeField] PlayerInput input;

    private ItemSlot[] slots;       // UI slots to interface with
    private RuneSlot[] runeSlots;
    private List<ItemData> items;   // The actual items the player has

    public int RuneDust 
    { 
        get { return runeDust; } 
        set { runeDust = value; 
        if (dustText != null)
            {
                dustText.text = runeDust.ToString();        // This method of the accessor calling the functionality might work, can add upper limits (max values to limit set value)
            }
        } 
    }
    private int runeDust;

    public int WoodAmount
    { 
        get { return woodAmount; } 
        set { woodAmount = value; 
        if (woodText != null)
            {
                woodText.text = woodAmount.ToString();
            }
        } 
    }
    
    private int woodAmount;

    public int StoneAmount
    { 
        get { return stoneAmount; } 
        set { stoneAmount = value; 
        if (stoneText != null)
            {
                stoneText.text = stoneAmount.ToString();
            }
        } 
    }
    private int stoneAmount;

    // input handling
    bool held = false;
    private ItemSlot sourceSlot;
    private GameObject ghost;
    private ItemData ghostItem;
    Vector2 mousePos;

    // Event System
    public delegate void InventoryEvent(InventoryEventArgs e);
    public event InventoryEvent OnItemReleased;
    public void FireReleasedItem(InventoryEventArgs e) { if (OnItemReleased != null) OnItemReleased(e); }

    // Per play inventory, this system needs to know when a player exists
    // It also needs to maintain relative data for each class, handling their individual inventories
    public void SetupInventory()
    {
        InitUISlots();
        InitItemList();
        MapActions();
    }

    public void SetupInventory(PlayerController controller)
    {
        input = controller.Input;
        SetupInventory();
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
            Vector2Int index = new Vector2Int(row, col);
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

            //slot.temp.text = slot.index.x + " : " + slot.index.y;
        }

        // Get a reference to the rune slots if they do not exist.
        if (runeSlots == null)
            runeSlots = inventoryScreen.GetComponentsInChildren<RuneSlot>();
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
        foreach (ItemSlot slot in runeSlots)
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
                if (slot is RuneSlot runeSlot)
                {
                    runeSlot.RemoveRuneEffects();
                }
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

        ItemSlot ghostSlot = ghost.GetComponent<ItemSlot>();
        ghostSlot.img.sprite = sourceSlot.img.sprite;
        ghostSlot.SlotSelected(true);

        //Image gImage = ghost.GetComponent<Image>();
        //gImage.sprite = sourceSlot.img.sprite;

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
                    // Dropped on a position that is not the same slot, or doesn't have an item
                    // Therefore, check if the slot is a rune, and if it is the matching type as the slot
                    // We don't want to put defense runes into the slots that are not allowed to contain defense runes

                    if (slot is RuneSlot runeSlot)              // If the slot is a rune slot
                    {
                        if (ghostItem.type == ItemTypes.RUNE && ghostItem is RuneData rune)   // If the held item is a rune
                        {
                            if (rune.runeType == runeSlot.type) // If the rune type matches the slot
                            {
                                Debug.Log("[Inventory]: Matching slots, embed the rune");
                                slot.item = ghostItem;
                                ghostItem.LoadSpriteToImage(slot.img);

                                // Tell the slot to use the rune
                                runeSlot.ApplyRuneEffects();
                            }
                            else
                            {
                                Debug.Log("[Inventory]: Slot is for " + runeSlot.type + " defaulting");
                                sourceSlot.item = ghostItem;
                                ghostItem.LoadSpriteToImage(sourceSlot.img);
                            }
                        }
                        else // The held item is not a rune
                        {
                            Debug.Log("[Inventory]: Failed placement, reverting");
                            // Revert item back to slot
                            sourceSlot.item = ghostItem;
                            ghostItem.LoadSpriteToImage(sourceSlot.img);
                        }
                    }
                    else // the selected slot is NOT a rune slot, handle regular placement.
                    {
                        Debug.Log("[Inventory]: Index (" + slot.index.x + " : " + slot.index.y + ") is empty, placing item here");
                        slot.item = ghostItem;
                        ghostItem.LoadSpriteToImage(slot.img);
                    }
                }
                ghostItem = null;

                InventoryEventArgs args = new InventoryEventArgs();
                args.data = slot.item;
                FireReleasedItem(args);
            }
            else
            {
                Debug.Log("[Inventory]: Did not release mouse on a valid position, reverting");

                // If release position is outside of the canvas --> we can detect to drop the item / discard
                sourceSlot.item = ghostItem;
                ghostItem.LoadSpriteToImage(sourceSlot.img);
            }
        }


        // NOTE: ISSUE WITH GHOST PLACEMENTS, STUCK IN CURSOR ON INVALID POSITIONS
        if (ghost)
        {
            Destroy(ghost);
        }
    }

    // private void UpdateResourceText(ResourceTypes resource)
    // {
    //     switch(resource)
    //     {
    //         case ResourceTypes.DUST:
    //             dustText.text = runeDust.ToString();
    //         break;
    //         case ResourceTypes.WOOD:
    //             woodText
    //         break;
    //         case ResourceTypes.STONE:

    //         break;
    //     }
    // }
}
