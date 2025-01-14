using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public enum PoolTypes { PROJECTILE }
    public static ObjectPoolManager Instance { get { return instance; } }
    private static ObjectPoolManager instance;

    [SerializeField] ObjectPool[] objPools;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetPooledObject(PoolTypes type)
    {
        return objPools[(int)type].GetPooledObject();   // Get a pooled object from the specified pool
    }

    public void Reset()
    {
        for (int i = 0; i < objPools.Length; i++)
        {
            objPools[i].Reset();
        }
    }
}
