using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance 
    { 
        get 
        { 
            if (instance == null)
                instance = FindObjectOfType<GameManager>();

            if (!instance)
                Debug.LogError("[GameManager]: No Game Manager exists!");

            return instance; 
        } 
    }

    public static PlayerController controller;
    public enum GameState { MENU, PLAYING, PAUSE }
    public GameState currGameState;

    public enum eLevel { HUB, LEVEL_1, LEVEL_2, LEVEL_3 }
    public eLevel level;

    public enum ControlType {  MOUSE_KEYBOARD, CONTROLLER }
    public ControlType controls;

    public PlayerMovement.MoveInput moveType;

#region EVENTS
    public delegate void moveChanged(PlayerMovement.MoveInput value);
    public event moveChanged onMoveChanged;
    void FireMoveChanged (PlayerMovement.MoveInput value) { if (onMoveChanged != null) onMoveChanged(value); }
#endregion

#region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (controller == null)
        { 
            controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        if (controller != null)
        { 
            moveType = controller.Movement.moveType;
        }
    }
#endregion
#region LEVEL LOADING
    private bool isLoading = false;
    LoadingScreen loadingScreen;

    // THIS LEVEL LOAD NEEDS TO BE TESTED, IT MAKES USE OF SCENE REFERENCES RATHER THAN STRING NAMES
    private IEnumerator LoadLevel(Scene toLoad)
    {
        isLoading = true;
        if (loadingScreen)
            loadingScreen.gameObject.SetActive(isLoading);
        if (pauseMenu)
            pauseMenu.CanPause = false; // Do not allow pauses to happen while loading
        
        if (SceneManager.GetActiveScene().isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            while (!asyncUnload.isDone)
                yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(toLoad.name, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            loadingScreen.UpdateSlider(asyncLoad.progress);
            yield return null;
        }

        SceneManager.SetActiveScene(toLoad);
        isLoading = false;

        if (pauseMenu)
            pauseMenu.CanPause = true;
        if (loadingScreen)
            loadingScreen.gameObject.SetActive(isLoading);
    }
#endregion

#region GENERAL FUNCTIONALITY
    // Pause Controls
    PauseMenu pauseMenu;

    // Exit Game
    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
    #else
        Application.Quit();
    #endif
    }
#endregion
}
