using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] targets; // a list of targets for enemies to follow - forward planning for coop
    // enemies will path towards the target closest to them
    // when calculating distance, use the square magnitude of the vectors so the square root function (expensive) is not being called especially per frame -- unless needed

    [SerializeField]
    GameObject enemyPrefab;

    List<GameObject> enemies = new List<GameObject>();

    [SerializeField]
    private int enemyCount = 100;    // the number of enemies to spawn

    public int currentEnemies = 0;

    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        if (targets.Length > 0)
            active = true;

        SpawnEnemies();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    // only handle logic if there are any players to reference and track
    //    if (active)
    //    {
    //
    //    }
    //}

    void SpawnEnemies()
    {
        if (enemyPrefab != null)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                GameObject temp = GameObject.Instantiate(enemyPrefab);
                enemies.Add(temp);
                currentEnemies++;
            }
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "Spawn enemies"))
        {
            SpawnEnemies();
        }
        GUI.Label(new Rect(10, 60, 100, 20), "Enemies: " + currentEnemies);
    }
}
