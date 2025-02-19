using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
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


    // Multiplicative, keep default of 1
    [Header("Balancing Values")]
    [Range(1, 2)] public float MainConvert = 1.012f;
    [Range(1, 2)] public float ArmourConvert = 0.05f;

    public PlayerController Player { get { return controller; } }
    [SerializeField] private PlayerController controller;

    public AudioSource GlobalAudio { get { return audioSource; } }
    [SerializeField] private AudioSource audioSource;

    // Get a reference to the hud
    public HUDController HUD { get { return hud; } }
    [SerializeField] private HUDController hud;
    public enum GameState { MENU, SELECT, LOADING, PLAYING, PAUSE }
    public GameState currGameState;
    public enum eLevel { MENU, HUB, LEVEL_1, LEVEL_2, LEVEL_3 }
    [SerializeField] string[] levelNames; // Map this in order of the types
    public eLevel level;
    private string currentLevelName;

    // SCENES
    public enum ControlType { MOUSE_KEYBOARD, CONTROLLER }
    public ControlType controls;
    public PlayerMovement.MoveInput moveType;
    #endregion

    #region EVENTS



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
        if (hud == null)
        {
            hud = FindObjectOfType<HUDController>();
        }
#if UNITY_EDITOR
        // Check for playmode override
        bool playOverride = EditorPrefs.GetBool("shouldOverride");
        Debug.Log(playOverride);
        if (!playOverride)
        {
            LevelLoad();
        }
#endif
#if UNITY_STANDALONE && !UNITY_EDITOR
        LevelLoad();
#endif
    }
    #endregion
    #region GAMEPLAY
    public void PlayerRespawn()
    {
        GameObject player = controller.gameObject;
        if (LevelManager.Instance.PlayerSpawnPoint != null)
        {
            player.transform.position = LevelManager.Instance.PlayerSpawnPoint.position;
            player.transform.rotation = LevelManager.Instance.PlayerSpawnPoint.rotation;
            controller.Movement.Respawn();
        }
        // Set the camera to follow
        LevelManager.Instance.LevelLoaded();

        // Load the camera into the hud for inventory sliding
        //hud.SetCamera(LevelManager.Instance.LevelCamera);

        // Allow the player to move

        // Reset the pools

        // Fixing UI displays.
    }
    #endregion

    #region LEVEL LOADING
    private bool isLoading = false;
    [SerializeField] LoadingScreen loadingScreen;

    // THIS LEVEL LOAD NEEDS TO BE TESTED, IT MAKES USE OF SCENE REFERENCES RATHER THAN STRING NAMES

    // Wrapper function
    public void LoadLevelFromString(string toLoad)
    {
        Debug.Log("[GameManager]: Starting to load: " + toLoad);
        StartCoroutine(LoadLevel(toLoad));
    }
    private IEnumerator LoadLevel(string levelName)
    {
        //Scene toLoad = SceneManager.GetSceneByBuildIndex(index);
        //Debug.Log(toLoad.name);

        isLoading = true;
        currGameState = GameState.LOADING;
        if (loadingScreen)
            loadingScreen.gameObject.SetActive(isLoading);
        if (pauseMenu)
            pauseMenu.CanPause = false; // Do not allow pauses to happen while loading

        if (AIManager.Instance)
            AIManager.Instance.LevelUnloading();
        if (DestructibleManager.Instance)
            DestructibleManager.Instance.LevelUnloading();

        // Only want to unload if the scene is not the persistent scene
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);
            while (!asyncUnload.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            if (loadingScreen)
                loadingScreen.UpdateSlider(asyncLoad.progress);
            yield return null;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LevelLoading();
            if (AIManager.Instance)
            {
                AIManager.Instance.LevelLoading();
            }
            if (DestructibleManager.Instance)
            {
                DestructibleManager.Instance.LevelLoading();
            }
        }


        currentLevelName = levelName;
        isLoading = false;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        if (pauseMenu)
            pauseMenu.CanPause = true;
        if (loadingScreen)
            loadingScreen.gameObject.SetActive(isLoading);

        currGameState = GameState.PLAYING;

        // If the level manager exits, do the stuff
        if (LevelManager.Instance)
        {
            // Load the player
            PlayerRespawn();
        }
    }

    public void LevelLoad()
    {
        //StartCoroutine(LoadLevel(levelNames[(int)level]));
        LoadLevelFromString((levelNames[(int)level]));
    }
    public void LoadMainMenu()
    {
        //StartCoroutine(LoadLevel(1);
        LoadLevelFromString((levelNames[(int)eLevel.MENU]));
    }

    public void LoadHub()
    {
        LoadLevelFromString((levelNames[(int)eLevel.HUB]));
    }

    public void LoadPrototype()
    {
        StartCoroutine(LoadLevel("Prototyping"));
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
