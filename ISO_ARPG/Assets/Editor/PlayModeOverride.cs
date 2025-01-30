using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.EditorCoroutines.Editor;

[InitializeOnLoad]
public class PlayModeOverride
{
    private const string DefaultScene = "Assets/0_Scenes/GoX Persistent Scene.unity";
    static PlayModeOverride()
    {
        EditorApplication.playModeStateChanged += PlayChanged;
    } 

    static Scene toLoad;
    private static void PlayChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // When we enter playmode, cache the scene that is expected to load,
            // Load persistent scene, and then load the expected scene
            toLoad = EditorSceneManager.GetActiveScene();
            Debug.Log(toLoad.name);
            if (toLoad.path != DefaultScene)
            {
                Debug.Log("Auto-loading default");

                EditorCoroutineUtility.StartCoroutineOwnerless(LoadScene(toLoad.name));
            }
        }
    }

    private static IEnumerator LoadScene(string name)
    {
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(0);
        while (!loadAsync.isDone)
        {
            yield return null;
        }
        GameManager.Instance.LoadLevelFromString(name);
    }
}
