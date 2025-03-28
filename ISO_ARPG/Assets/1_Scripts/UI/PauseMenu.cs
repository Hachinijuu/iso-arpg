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
            if (canPause && GameManager.Instance.currGameState != GameManager.GameState.MENU && GameManager.Instance.level != GameManager.eLevel.CUTSCENE && GameManager.Instance.level != GameManager.eLevel.TRANSITION)
            {
                if (!panel.activeInHierarchy && !settingsOn)
                {
                    GameManager.Instance.PauseGame();
                    panel.SetActive(true);
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