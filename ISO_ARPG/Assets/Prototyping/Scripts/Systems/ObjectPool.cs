using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance {get {return instance;}}
    private static ObjectPool instance;

    [SerializeField] GameObject pooledPrefab;
    public List<GameObject> pooledObjects;
    [SerializeField] int numPooledObjects = 5;

    [SerializeField] bool canGrow = false;

    private void Awake()
    {
        instance = this;
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