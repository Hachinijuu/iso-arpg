using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatScreen : MonoBehaviour
{
    public void ReturnToHubClicked()
    {
        // Only load if not already in hub
        if (GameManager.Instance.level != GameManager.eLevel.HUB)
        {
            GameManager.Instance.LoadHub();
            GameManager.Instance.UnpauseGame();
            gameObject.SetActive(false);
        }
    }

    public void ReturnToMainMenuClicked()
    {
        GameManager.Instance.LoadMainMenu();
        GameManager.Instance.UnpauseGame();
        gameObject.SetActive(false);
    }

    public void QuitClicked()
    {
        GameManager.Instance.QuitGame();
        GameManager.Instance.UnpauseGame();
    }
}
