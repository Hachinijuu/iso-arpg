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
            // Dirty solution fix later
            //if (!GameManager.Instance.CutscenePlayed)
            //{
            //    GameManager.Instance.CutscenePlayed = true;
            //    GameManager.Instance.LoadLevelByID(GameManager.eLevel.CUTSCENE);
            //    return;
            //}

            if (levelList == null)
            {
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
