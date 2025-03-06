using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{

    // MAY GET RID OF THIS CLASS AND HAVE EACH RESPECTIVE MANAGER HANDLER THEIR OWN OBJECT POOLS
    // PROBABLY MORE ORGANIZED TO STRUCTURE WITH RESPECTIVE POOLS

    // THIS MANAGER WILL HANDLE THE GENERAL OBJECT POOLS
    // PROJECTILES AND EFFECTS MAYBE
    public enum PoolTypes { ARROW_PROJECTILE, FIREBALL, AXE_PROJECTILE }
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
