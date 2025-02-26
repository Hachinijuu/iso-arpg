using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemySpawnPoint : MonoBehaviour
{
    public float minSpawnDistance = 5;
    public float maxSpawnDistance = 10;

    // Within the donut of min and max spawn points are the eligble positions the enemies can spawn at this point
    // Start is called before the first frame update
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemySpawnPoint))]
public class SpawnPointEditor : Editor
{
    public void OnSceneGUI()
    {
        EnemySpawnPoint spawnPoint = target as EnemySpawnPoint;
        Handles.color = Color.cyan;
        EditorGUI.BeginChangeCheck();
        float minRange = Handles.RadiusHandle(Quaternion.identity, spawnPoint.transform.position, spawnPoint.minSpawnDistance);
        float maxRange = Handles.RadiusHandle(Quaternion.identity, spawnPoint.transform.position, spawnPoint.maxSpawnDistance);
        if (EditorGUI.EndChangeCheck())
        {
           Undo.RecordObject(target, "Update Range");
           spawnPoint.minSpawnDistance = minRange;
           spawnPoint.maxSpawnDistance = maxRange;
        }
    }
}
#endif
