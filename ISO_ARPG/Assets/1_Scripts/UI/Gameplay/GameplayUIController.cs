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
    [SerializeField] public GameObject charScreens;
    [SerializeField] public HUDController hud;
    [SerializeField] public GameObject statsScreen;
    [SerializeField] public GameObject inventoryScreen;
    [SerializeField] public GameObject stashScreen;
    public PauseMenu pauseScreen;
    [SerializeField] public RuneUpgradeScreen smithScreen;
    [SerializeField] public SettingsController settings;

    public GameObject minimap;
    public GameObject objectiveText;

    public void ShowGameIndicators(bool on)
    {
        minimap.SetActive(on);
        if (LevelManager.Instance != null && LevelManager.Instance.type != LevelManager.LevelType.NONE)
        {
            objectiveText.SetActive(on);
        }
        else
        {
            objectiveText.SetActive(false);
        }
    }

    private void OnEnable()
    {
        TurnOnRelevantElements();
        charScreens.SetActive(true);
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

    public void ShowInventory()
    {
        Inventory.Instance.ShowInventory();
    }

    public void ShowCharacterScreen()
    {
        Inventory.Instance.ShowCharacterScreen();
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
        if (GameManager.Instance.currGameState == GameManager.GameState.PAUSE)  // Force stop for edge cases
        {
            GameManager.Instance.UnpauseGame();
        }
    }

    public void ShowStats()
    {
        statsScreen.SetActive(false);
    }

    public void HideStats()
    {
        statsScreen.SetActive(false);
    }

    public void ShowSettings()
    {
        pauseScreen.SettingsClicked();
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
