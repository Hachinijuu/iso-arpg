using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtonLoader : MonoBehaviour
{
    public void LoadLevel1()
    {
        GameManager.Instance.LoadLevelByID(GameManager.eLevel.LEVEL_1);
    }

    public void LoadLevel2() 
    { 
        GameManager.Instance.LoadLevelByID(GameManager.eLevel.LEVEL_2);
    }

    public void LoadLevel3()
    { 
        GameManager.Instance.LoadLevelByID(GameManager.eLevel.LEVEL_3);
    }
}
