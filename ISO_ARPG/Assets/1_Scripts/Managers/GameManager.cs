using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
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

    [Header("Global References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PlayerController controller;
    public PlayerController Player { get { return controller; } }
    public AudioSource GlobalAudio { get { return audioSource; } }

    // Get a reference to the hud
    //public HUDController HUD { get { return hud; } }
    //[SerializeField] private HUDController hud;
    [Header("States")]
    public GameState currGameState;
    public enum GameState { MENU, SELECT, LOADING, PLAYING, PAUSE }
    public enum eLevel { MENU, HUB, CUTSCENE, LEVEL_1, TRANSITION, LEVEL_2, LEVEL_3 }
    [Header("Level Information")]
    [SerializeField] string[] levelNames; // Map this in order of the types
    public eLevel level;
    private string currentLevelName;
    public GameObject GameUI;
    [SerializeField] public DefeatScreen loseScreen;
    public bool CutscenePlayed = false;

    // SCENES
    [Header("Control Schemes")]
    public ControlType controls;
    public enum ControlType { MOUSE_KEYBOARD, CONTROLLER }
    [Header("Difficulty Settings")]
    public DifficultySetting[] difficulties;
    public DifficultySetting currDifficulty;
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
        // CONTROLLER WILL NOT BE FOUND ON START, IT WILL BE ASSIGNED AFTER THE PLAYER HAS BEEN SELECTED

        //if (controller == null)
        //{
        //    controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //}
#if UNITY_EDITOR
        // Check for playmode override
        bool playOverride = EditorPrefs.GetBool("shouldOverride");
        Debug.Log(playOverride);
        if (!playOverride)
        {
            LoadMainMenu();
        }
#endif
#if UNITY_STANDALONE && !UNITY_EDITOR
    LoadMainMenu();
#endif
    }
    #endregion
    #region GAMEPLAY
    public void PlayerLoading()
    {
        // When the player is loading, position them where they should be in the level
        if (controller == null)
        {
            controller = PlayerManager.Instance.currentPlayer;
        }
        if (controller != null)
        {
            GameObject player = controller.gameObject;
            if (LevelManager.Instance.PlayerSpawnPoint != null)
            {
                player.transform.position = LevelManager.Instance.PlayerSpawnPoint.position;
                player.transform.rotation = LevelManager.Instance.PlayerSpawnPoint.rotation;
            }
        controller.FootSteps.SetMaterial(LevelManager.Instance.material);
        }
        else
        {
            Debug.LogWarning("[GameManager]: No player to load");
        }

        //GameObject player = controller.gameObject;

        // Set the camera to follow
        //LevelManager.Instance.LevelLoaded();

        //if (controller.Stats.Health)

        // Load the camera into the hud for inventory sliding
        //hud.SetCamera(LevelManager.Instance.LevelCamera);

        // Allow the player to move

        // Reset the pools

        // Fixing UI displays.
    }
    public void PlayerRespawn()
    {
        // If the player that should be respawned is not already active in the hierarchy
        if (controller != null)
        {
            if (!controller.isActiveAndEnabled)
            {
                controller.gameObject.SetActive(true);
            }
            PlayerLoading();
            LevelManager.Instance.LevelLoaded();
            controller.Respawn();   // Tell the player to respawn themselves
        }
        else
        {
            Debug.LogWarning("[GameManager]: No player to respawn");
        }
    }

    public void PlayerRespawn(PlayerController player)
    {
        controller = player;
        PlayerRespawn();
    }

    public void PlayerDied()
    {
        // Display death / defeat screen and play death music
        if (!loseScreen.gameObject.activeInHierarchy)
        {
            controller.Died();
            // Play dead sounds and death animation, could keep game unpaused
            loseScreen.gameObject.SetActive(true);
            PauseGame();
        }
        // Upon pressing return to hub, go back to the hub
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
        currGameState = GameState.LOADING; // block player input while in loading state
        GameUI.SetActive(false);
        if (loadingScreen)
            loadingScreen.gameObject.SetActive(isLoading);
        if (pauseMenu)
            pauseMenu.CanPause = false; // Do not allow pauses to happen while loading

        // In the loading state, disable player components
        if (controller != null)
            controller.EnablePlayer(false);

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
            PlayerLoading();
        }


        currentLevelName = levelName;
        level = GetLevelFromName(levelName);
        isLoading = false;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        if (pauseMenu)
            pauseMenu.CanPause = true;
        if (loadingScreen)
            loadingScreen.gameObject.SetActive(isLoading);
        


        // If the level manager exits, do the stuff
        if (LevelManager.Instance)
        {
            currGameState = GameState.PLAYING;
            // Load the player
            GameUI.SetActive(true);
            if (controller)         // If the player exists, load the player -- there should be no player by default
            {
                PlayerRespawn();
                // Set the camera to follow
            }
        }
    }

    public eLevel GetLevelFromName(string levelName)
    {
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (levelNames[i] == levelName)
                return (eLevel)i;
        }
        return eLevel.MENU; // Default is menu
    }

    public void LevelLoad()
    {
        //StartCoroutine(LoadLevel(levelNames[(int)level]));
        LoadLevelFromString((levelNames[(int)level]));
    }

    public void LoadLevelByID(eLevel level)
    {
        LoadLevelFromString((levelNames[(int)level]));
    }
    public void LoadMainMenu()
    {
        //StartCoroutine(LoadLevel(1);
        LoadLevelFromString((levelNames[(int)eLevel.MENU]));
        currGameState = GameState.MENU;
    }

    public void LoadHub()
    {
        LoadLevelFromString((levelNames[(int)eLevel.HUB]));

        // Special conditions to check when loading to the hub
        if (PlayerManager.Instance.currentClass == GoX_Class.NONE)
        {
            // If the player has no class when loading the hub
            // We want to load the hub setup for character selection
        }
    }

    public void LoadPrototype()
    {
        StartCoroutine(LoadLevel("Prototyping"));
    }
    #endregion

    #region GENERAL FUNCTIONALITY
    // Pause Controls
    [SerializeField] PauseMenu pauseMenu;

    public void PauseGame()
    {
        currGameState = GameState.PAUSE;
        Time.timeScale = 0.0f;
    }

    public void UnpauseGame()
    {
        currGameState = GameState.PLAYING;
        Time.timeScale = 1.0f;
    }

    public void GetPlayerReferences()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (controller != null)
        {
            
        }
        PlayerLoading();
        PlayerRespawn();
    }

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
