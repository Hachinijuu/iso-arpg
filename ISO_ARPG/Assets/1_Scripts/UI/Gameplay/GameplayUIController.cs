using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    private static GameplayUIController instance = null;
    public static GameplayUIController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameplayUIController>();

            if (!instance)
                Debug.LogError("[GameplayUIController]: No GameUIController exists!");

            return instance;
        }
    }

    // This has references to the hud elements and disable or enable accordingly
    [SerializeField] public HUDController hud;
    [SerializeField] public GameObject statsScreen;
    [SerializeField] public GameObject inventoryScreen;
    [SerializeField] public GameObject stashScreen;
    [SerializeField] public RuneUpgradeScreen smithScreen;

    private void OnEnable()
    {
        TurnOnRelevantElements();
    }

    private void OnDisable()
    {
        ShutoffElements();
    }

    // On load cleanup, chut off character screens....?
    public void ShutoffElements()
    {
        //hud.gameObject.SetActive(false);
        statsScreen.SetActive(false);
        inventoryScreen.SetActive(false);
        UIUtility.FlushUIElements();
        //Inventory.Instance.ShowCharacterScreen();
        //Inventory.Instance.ShowInventory();
        stashScreen.SetActive(false);
        smithScreen.gameObject.SetActive(false);
        GameManager.Instance.FixCamera();
    }

    public void TurnOnRelevantElements()
    {
        hud.gameObject.SetActive(true);
    }

    public void ShowSmith()
    {
        //Debug.Log("Showing smithing screen");
        smithScreen.gameObject.SetActive(true);
    }

    public void HideSmith()
    {
        //Debug.Log("Hiding smithing screen");
        smithScreen.gameObject.SetActive(false);
    }

    public void ShowStats()
    {
        statsScreen.SetActive(false);
    }

    public void HideStats()
    {
        statsScreen.SetActive(false);
    }


    // Find an inexpensive way to handle hovers, probably back to mouse detection / raycasts for physics layers

    public delegate void MouseHoverEnter(MouseHoverEventArgs e);
    public event MouseHoverEnter OnHoverEnter;

    public delegate void MouseHoverExit(MouseHoverEventArgs e);
    public event MouseHoverExit OnHoverExit;

    // These events are fired by the relative targets that need to handle hovering
    public void FireMouseHovered(MouseHoverEventArgs e)
    {
        // HUD listens to the mouse hovered event
        if (OnHoverEnter != null) { OnHoverEnter(e); }
    }

    public void FireMouseExit(MouseHoverEventArgs e)
    {
        if (OnHoverExit != null) { OnHoverExit(new MouseHoverEventArgs(null)); }    // When mouse exits a valid hover, FORCE to null
    }


    [SerializeField] GameObject infoTooltip;    // This is for displaying general information -- What does an ability do?
    [SerializeField] GameObject runeTooltip;    // This is for displaying information that is RUNE data
    // TOOLTIPS
    GameObject tooltip;
    public void CreateRuneTooltip(ToolTipArgs e)
    {
        tipActive = e.over;
        if (tipActive && tooltip == null)
        {
            StartCoroutine(HandleRuneTooltip(e));
        }
    }

    bool tipActive;

    // The p
    IEnumerator HandleRuneTooltip(ToolTipArgs e)
    {
        tooltip = Instantiate(runeTooltip, inventoryScreen.transform);   // Parent it to the inventory
        RectTransform rect = tooltip.GetComponent<RectTransform>();
        RuneTooltip rt = tooltip.GetComponent<RuneTooltip>();       
        
        if (!(e.item is RuneData)) { yield break; } // If the item is not rune data, eject
        if (rt != null)
        {
            rect.position = e.screenPos;
            rt.SetRuneData(e.item as RuneData);
        }

        do
        {
            yield return new WaitForEndOfFrame();
        } while (tipActive);
        Destroy(tooltip);
    }
}

public class MouseHoverEventArgs
{
    public GameObject hovered;

    public MouseHoverEventArgs(GameObject go)
    {
        hovered = go;
    }
}

public class ToolTipArgs
{
    public Vector2 screenPos;
    public bool over;
    public ItemData item;
}
