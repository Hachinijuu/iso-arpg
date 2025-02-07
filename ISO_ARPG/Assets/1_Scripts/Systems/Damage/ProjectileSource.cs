using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSource : MonoBehaviour
{
    [SerializeField] Transform[] firePositions;
    public void GetPooledProjectile(ObjectPoolManager.PoolTypes type, int posIndex)
    {
        GameObject projectile = ObjectPoolManager.Instance.GetPooledObject(type);
        if (projectile != null)
        {
            Transform pos = firePositions[posIndex];
            projectile.transform.position = pos.position;
            projectile.transform.rotation = pos.rotation;
            if (!(projectile.activeInHierarchy))
            {
                projectile.SetActive(true);
            }
        }
    }
}
