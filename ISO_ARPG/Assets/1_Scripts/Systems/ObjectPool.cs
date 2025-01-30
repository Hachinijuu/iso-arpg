using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{//
    //public static ObjectPool Instance { get { return instance; } }
    //private static ObjectPool instance;
    public GameObject Prefab { get { return pooledPrefab; } } 
    [SerializeField] GameObject pooledPrefab;
    public List<GameObject> pooledObjects;
    [SerializeField] int minPooledObjects = 2;
    [SerializeField] int numPooledObjects = 5;
    public bool CanGrow { get {return canGrow; } set { canGrow = value; } }
    [SerializeField] bool canGrow = false;

    private void Awake()
    {
        // /instance = this;
    }

    private void Start()
    {
        FillPool();
    }

    public void FillPool()
    {
        for (int i = 0; i < numPooledObjects; i++)
        {
            CreatePooledObject();
        }
    }

    public void GrowPool(int toAdd)
    {
        canGrow = true;
        for (int i = 0; i < toAdd; i++)
        {
            CreatePooledObject();
        }
        canGrow = false;
    }
    public void ShrinkPool(int toRemove)
    {
        if (toRemove > pooledObjects.Count - minPooledObjects)  // If the requested amount to remove would bleed into the minimum
        {
            // Calculate the amount to remove to meet the minimum
            toRemove = pooledObjects.Count - minPooledObjects; // What I have - Amount I need = How much I can take to be left with what I need // 10 - 2 = 8, I can take 8 away from 10 to be left with 2
        }
        pooledObjects.RemoveRange(pooledObjects.Count - toRemove, toRemove);    // Index to start from is the end - the amount to remove, remove the amount
    }

    GameObject CreatePooledObject()
    {
        GameObject obj = Instantiate(pooledPrefab); // Instantiate the object based on the given prefab
        obj.SetActive(false);                       // Set the object to false so it is not visible
        obj.transform.SetParent(transform);         // Make the created object a child of the pool
        pooledObjects.Add(obj);                     // Add the object to the list
        return obj;                                 // Return the object that was just created
    }

    public GameObject GetPooledObject()
    {
        GameObject obj = null;
        foreach (GameObject pObj in pooledObjects)
        {
            if (!pObj.gameObject.activeInHierarchy) // If the object is NOT active in the hierarchy (unused)
            {
                obj = pObj; // Get the object that is not being used, to be returned
                break;
            }
        }
        
        if (!obj)   // Once the loop has been complete, check if the object exists
        {
            if (canGrow)    // If the pool can grow
                obj = CreatePooledObject(); // Add another object
            else
                Debug.LogWarning("[ObjectPool]: " + pooledPrefab.name + " does not have an available object");
        }
        return obj;
    }

    public void Reset()
    {
        // Deactivate all the objects in the pool
        foreach(GameObject pObj in pooledObjects)
        {
            pObj.SetActive(false);
        }
    }
}