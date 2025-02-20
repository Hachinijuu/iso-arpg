using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelVolume : MonoBehaviour
{
    public GameManager.eLevel toLoad;
    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Collided with " + other.name);
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.level = toLoad;
            GameManager.Instance.LevelLoad();
        }
    }
}
