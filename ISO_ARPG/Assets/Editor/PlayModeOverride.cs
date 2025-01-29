using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;

[InitializeOnLoad]
public class PlayModeOverride
{
    //private const string DefaultScene = "Assets/0_Scenes/GoX Persistent Scene.unity";
    //static PlayModeOverride()
    //{
    //    EditorApplication.playModeStateChanged += PlayChanged;
    //} 
//
    //private static void PlayChanged(PlayModeStateChange state)
    //{
    //    if (state == PlayModeStateChange.EnteredPlayMode)
    //    {
    //        // When we enter playmode, cache the scene that is expected to load,
    //        // Load persistent scene, and then load the expected scene
    //        Scene toLoad = EditorSceneManager.GetActiveScene();
    //        Debug.Log(toLoad.name);
    //        if (toLoad.path != DefaultScene)
    //        {
    //            Debug.Log("Auto-loading default");
//
    //            SceneManager.LoadScene(0);
//
    //            SceneManager.SetActiveScene()
//
    //            GameManager.Instance.LoadLevelFromString(toLoad.name);
    //            //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);
    //            //if (asyncLoad.isDone)
    //            //{
    //            //    Debug.Log("Finished loading");
    //            //    GameManager.Instance.LoadLevelFromString(toLoad.name);
    //            //}
    //        }
    //    }
    //}
}
