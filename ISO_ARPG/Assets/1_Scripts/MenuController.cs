using UnityEngine;

public class MenuController : MonoBehaviour
{

    public void StartClicked()
    {
        // Load the hub, character selection
        GameManager.Instance.LoadCharacterSelect();
    }

    public void ContinueClicked()
    {
        // Load the hub, preloaded character from previous save file
        GameManager.Instance.LoadHub();
    }

    public void SettingClicked()
    {
        // open up the settings screen
        GameManager.Instance.settingsScreen.settingsScreen.SetActive(true);
    }

    public void CreditsClicked()
    {
        // Open up a credits screen, same scene (menu bound)
    }

    public void QuitClicked()
    {
        // We can prompt a leave, and then quit, for now just leave.
        GameManager.Instance.QuitGame();
    }

    public void PlayAudio(AudioClip clip)
    {
        UIAudioHandler.Instance.PlayAudio(clip);
    }

}
