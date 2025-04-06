using UnityEngine;
using UnityEngine.Rendering;

public class PauseMenu : MonoBehaviour 
{
    [SerializeField] GameObject panel;
    public bool CanPause {get { return canPause; } set { canPause = true; } }
    private bool canPause = true;

    [SerializeField] GameObject settingsPanel;

    // refactor to new update system in time

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canPause && GameManager.Instance.currGameState != GameManager.GameState.MENU && 
                GameManager.Instance.level != GameManager.eLevel.CUTSCENE && 
                GameManager.Instance.level != GameManager.eLevel.TRANSITION)
            {
                if (!panel.activeInHierarchy && !settingsOn)
                {
                    if (GameManager.Instance.currGameState != GameManager.GameState.PAUSE)
                    {
                        if (GameplayUIController.Instance.inventoryScreen.gameObject.activeInHierarchy)
                        {
                            Inventory.Instance.ShowInventory();
                        }
                        else if (GameplayUIController.Instance.statsScreen.gameObject.activeInHierarchy)
                        {
                            Inventory.Instance.ShowCharacterScreen();
                        }
                        else if (settingsPanel.activeInHierarchy)
                        {
                            settingsPanel.SetActive(false);
                        }
                        else
                        {
                            GameManager.Instance.PauseGame();
                            panel.SetActive(true);
                        }
                    }
                    else
                    {
                        if (GameplayUIController.Instance.smithScreen.gameObject.activeInHierarchy) // If Hendok is open
                        {
                            Inventory.Instance.ShowInventory(); // This should fix inventory interaction
                            //GameplayUIController.Instance.HideSmith();
                        }
                        else if (GameplayUIController.Instance.inventoryScreen.gameObject.activeInHierarchy && GameplayUIController.Instance.statsScreen.activeInHierarchy)
                        {
                            Inventory.Instance.ShowCharacterScreen();
                            Inventory.Instance.ShowInventory();
                            // These will both handle the pause
                        }
                    }
                }
                else    // 2 step back out if settings is active (back button)
                {
                    if (settingsOn)
                    {
                        settingsPanel.SetActive(false);
                        settingsOn = false;
                    }
                    else
                    {
                        panel.SetActive(false);
                        GameManager.Instance.UnpauseGame();
                    }
                }
            }
        }
    }

    public void ContinueClicked()
    {
        panel.SetActive(false);
        GameManager.Instance.UnpauseGame();
    }

    bool settingsOn = false;
    public void SettingsClicked()
    {
        settingsPanel.SetActive(true);  // Turn settings on
        settingsOn = true;
        //panel.SetActive(false);         // Deactivate pause screen
    }

    public void ReturnToHubClicked()
    {
        // Only load if not already in hub
        if (GameManager.Instance.level != GameManager.eLevel.HUB)
        {
            panel.SetActive(false);
            GameManager.Instance.LoadHub();
            GameManager.Instance.UnpauseGame();
        }
    }

    public void ReturnToMainMenuClicked()
    {
        panel.SetActive(false);
        GameManager.Instance.LoadMainMenu();
        GameManager.Instance.UnpauseGame();
    }

    public void QuitClicked()
    {
        GameManager.Instance.QuitGame();
        GameManager.Instance.UnpauseGame();
    }
}