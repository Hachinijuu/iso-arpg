using UnityEngine;

public class LevelVolume : MonoBehaviour
{
    public GameObject levelList;    // Level List, 
    public GameManager.eLevel toLoad;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with " + other.name);
        if (other.CompareTag("Player"))
        {
            SaveSystem.Instance.SaveProfile();
            // Dirty solution fix later
            //if (!GameManager.Instance.CutscenePlayed)
            //{
            //    GameManager.Instance.CutscenePlayed = true;
            //    GameManager.Instance.LoadLevelByID(GameManager.eLevel.CUTSCENE);
            //    return;
            //}

            if (levelList == null)
            {

                // No level list, check if they've seen the transition selection

                if (toLoad == GameManager.eLevel.TRANSITION && SaveSystem.Instance.currentProfile.gameData.seenIdentity)
                {
                    // If the loader should load the transition, and the identity was already seen -- edge case scenario
                    int nextID = (int)GameManager.eLevel.TRANSITION;
                    nextID++;
                    GameManager.Instance.LevelLoad();
                    GameManager.Instance.level = (GameManager.eLevel)nextID;
                    return;
                }
                GameManager.Instance.level = toLoad;
                GameManager.Instance.LevelLoad();
            }
            else
            { 
                levelList.SetActive(true);
                GameManager.Instance.PauseGame();
            }
        }
    }
}
