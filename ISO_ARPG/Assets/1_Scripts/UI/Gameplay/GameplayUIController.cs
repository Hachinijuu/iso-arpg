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
    [SerializeField] HUDController hud;
    [SerializeField] GameObject statsScreen;
    [SerializeField] GameObject inventoryScreen;
    [SerializeField] GameObject stashScreen;
    [SerializeField] GameObject smithScreen;

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
        stashScreen.SetActive(false);
        smithScreen.SetActive(false);
    }

    public void TurnOnRelevantElements()
    {
        hud.gameObject.SetActive(true);
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
}

public class MouseHoverEventArgs
{
    public GameObject hovered;

    public MouseHoverEventArgs(GameObject go)
    {
        hovered = go;
    }
}
