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

    // damage = (stats.Damage.Value * stats.SecondaryDamage.Value) + (stats.STR.Value * GameManager.Instance.MainConvert);
    // float recalc = args.amount * Mathf.Clamp01(1.0f - ((stats.Armour.Value * GameManager.Instance.ArmourConvert) / 100));
    [Header("Balancing Values")]
    [Range(0, 2.0f)] public float MainConvert = 0.2f;
    [Range(0, 2.0f)] public float ArmourConvert = 0.05f;

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
    public enum eLevel { MENU, CHARACTER_LOAD, CHARACTER_SELECT, CUTSCENE, HUB, LEVEL_1, TRANSITION, LEVEL_2, LEVEL_3, PROTOTYPE }
    [Header("Level Information")]
    [SerializeField] string[] levelNames; // Map this in order of the types
    public eLevel level;
    private string currentLevelName;
    public GameObject GameUI;
    [SerializeField] HUDController hud;
    [SerializeField] public DefeatScreen loseScreen;
    [SerializeField] public SettingsController settingsScreen;
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
    public delegate void DifficultyChanged(DifficultySetting setting);
    public event DifficultyChanged onDifficultyChanged;
    public void FireDifficultyChanged(DifficultySetting setting) { if (onDifficultyChanged != null) onDifficultyChanged(setting); }
    public void FireDifficultyChanged() { if (onDifficultyChanged != null) onDifficultyChanged(currDifficulty); }

    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // CONTROLLER WILL NOT BE FOUND ON START, IT WILL BE ASSIGNED AFTER THE PLAYER HAS BEEN SELECTED
        currDifficulty = difficulties[0];

        //if (controller == null)
        //{
        //    controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //}
#if UNITY_EDITOR
        // Check for playmode override
        bool playOverride = EditorPrefs.GetBool("shouldOverride");
        //Debug.Log(playOverride);
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
            controller.Movement.HandleStops(true);    // Tell the movement to stop where they are
            Debug.Log("[GameManager]: Player Position: " + controller.transform.position + ", Level Spawn Point: " + LevelManager.Instance.PlayerSpawnPoint + ", Player Move Target: " + controller.Movement.MoveTarget);
            controller.Respawn();   // Tell the player to respawn themselves
            hud.Reload();
        }
        else
        {
            Debug.LogWarning("[GameManager]: No player to respawn");
        }
    }

    public void SetPlayer(PlayerController player)
    {
        controller = player;
    }
    public void PlayerRespawn(PlayerController player)
    {
        controller = player;
        PlayerRespawn();
    }

    public void PlayerDied()
    {
        // Display death / defeat screen and play death music
        Debug.Log("Player has died");
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
        {
            loadingScreen.gameObject.SetActive(isLoading);
            //loadingScreen.FadeIn();
        }
        if (pauseMenu)
            pauseMenu.CanPause = false; // Do not allow pauses to happen while loading

        // In the loading state, disable player components
        if (controller != null)
            controller.EnablePlayer(false);

        if (LevelManager.Instance)
        {
            hud.RemoveLevelObjective();
        }

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
            if (LevelManager.Instance.type == LevelManager.LevelType.CLEAR || LevelManager.Instance.type == LevelManager.LevelType.ELITE)
            {
                hud.ShowLevelObjectives();
            }
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
        {
            //loadingScreen.FadeOut();
            loadingScreen.gameObject.SetActive(isLoading);
        }

        FixCamera();
        // If the level manager exists, do the stuff
        if (LevelManager.Instance)  // ASSUME PLAYABLE IF A LEVEL MANAGER EXISTS
        {

            SetGameState(GameState.PLAYING);
            //currGameState = GameState.PLAYING;
            // Load the player
            GameUI.SetActive(true);
            // If the levelManager is NOT NONE condition, then play the battle music
            if (LevelManager.Instance.type == LevelManager.LevelType.CLEAR || LevelManager.Instance.type == LevelManager.LevelType.ELITE)
            {
                hud.ShowLevelObjectives();
                MusicManager.Instance.SetBattleMusic();
            }
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
        //LoadLevelFromString((levelNames[(int)eLevel.MENU]));
        LoadLevelByID(eLevel.MENU);
        currGameState = GameState.MENU;
        PlayerManager.Instance.DeactivatePlayer();
        MusicManager.Instance.SetTitleMusic();
        Inventory.Instance.CleanupInventory();
    }

    public void LoadHub()
    {
        MusicManager.Instance.SetHubMusic();
        LoadLevelByID(eLevel.HUB);
        //LoadLevelFromString((levelNames[(int)eLevel.HUB]));

        // Special conditions to check when loading to the hub
        // if (PlayerManager.Instance.currentClass == GoX_Class.NONE)
        // {
        //     // If the player has no class when loading the hub
        //     // We want to load the hub setup for character selection

        //     // Default the Camera to the view of the podium
        //     // Place the playable characters at the podium
        //     // Enable them accordingly
        //     //PlayerManager.Instance.HandlePlayerSelect();
        // }
    }

    public void HandleNewGame()
    {
        UIUtility.FlushUIElements();
    }

    public void LoadCharacterSelect()
    {
        LoadLevelByID(eLevel.CHARACTER_LOAD);
    }
    public void LoadNewCharacter()
    {
        //PlayerManager.Instance.DeactivatePlayer();
        HandleNewGame();
        PlayerManager.Instance.DeactivatePlayer();
        LoadLevelByID(eLevel.CHARACTER_SELECT);
        currGameState = GameState.SELECT;
        // Disable all players
        //PlayerManager.Instance.EnableCharacters(false); // Shut off the players for the ghost selection

        //LoadLevelFromString((levelNames[(int)eLevel.CHARACTER_SELECT]));
        // Special conditions to check when loading to the hub
        // if (PlayerManager.Instance.currentClass == GoX_Class.NONE)
        // {
        //     // If the player has no class when loading the hub
        //     // We want to load the hub setup for character selection

        //     // Default the Camera to the view of the podium
        //     // Place the playable characters at the podium
        //     // Enable them accordingly
        //     //PlayerManager.Instance.HandlePlayerSelect();
        // }
    }


    public void LoadPrototype()
    {
        StartCoroutine(LoadLevel("Prototyping"));
    }


    public void CoroutineStarter(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void FixCamera()
    {
        // This is to fix the camera after any loading issues / previous hud displays
        Camera cam = Camera.main;
        Rect defaultCam = new Rect(0,0, 1, 1);
        cam.rect = defaultCam;
    }

    #endregion

    #region GENERAL FUNCTIONALITY
    // Pause Controls
    [SerializeField] PauseMenu pauseMenu;

    GameState prevGameState;

    public void SetGameState(GameState newState)
    {
        prevGameState = currGameState;
        currGameState = newState;
    }

    public void PauseGame()
    {
        SetGameState(GameState.PAUSE);
        //currGameState = GameState.PAUSE;
        // For unpause and pause, revert back to whichever was the previous state

        Time.timeScale = 0.0f;
    }

    public void UnpauseGame()
    {
        SetGameState(prevGameState);
        //currGameState = GameState.PLAYING;
        Time.timeScale = 1.0f;
    }

    public void GetPlayerReferences()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (controller != null)
        {
            PlayerLoading();
            PlayerRespawn();
        }
    }

    public void PlayOneShotClip(AudioClip toPlay)
    { 
        audioSource.PlayOneShot(toPlay);
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
