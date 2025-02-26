using System.Collections;
using UnityEngine;

public class ProjectileSource : MonoBehaviour
{
    [SerializeField] Transform[] firePositions;
    [SerializeField] float offset = 2.0f;
    [SerializeField] float angleStep = 180.0f;
    [SerializeField] Transform fireLocation;

    // this should listen to stats, and init fire positions whenever the value of the projectiles changes
    // this will maintain updated projectile counts throughout the game, via gear and such

    // if no stats exists, the projectiles source will default to a predefined count outlined by the script itself

    public void Start()
    {
        if (fireLocation == null)
            fireLocation = transform;
        // Check if this script is attached to a player
        if (gameObject.tag == "Player")
        {
            // If it is, we want the fire positions to LISTEN to the number of projectiles, if the number of projectiles changes, then the number of projectiles sent out changes
            PlayerStats stats = GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Projectiles.Changed += context => { UpdateFirePositions((int)stats.Projectiles.Value); };
                if (stats.Projectiles.Value > 0)
                {
                    InitFirePositions((int)stats.Projectiles.Value);
                }
            }
        }
        else
        {
            InitFirePositions();
        }
    }

    public void UpdateFirePositions(int numPositions)
    {
        // If there are no fire positions already
        if (firePositions.Length <= 0 || firePositions == null)
        {
            InitFirePositions(numPositions);    // Create new fire positions
        }
        else
        {
            // resize and add more projectiles
            if (firePositions.Length > 0 && firePositions != null)
            {
                //Transform[] temp = new Transform[numPositions];
                CleanupPositions();
                firePositions = new Transform[numPositions];
                CreateProjectilePositions(numPositions);
            }
        }
    }

    public void CleanupPositions()
    {
        for (int i = 0; i < firePositions.Length; i++)
        {
            Destroy(firePositions[i].gameObject);
        }
        firePositions = null;
    }
    public void InitFirePositions(int numPositions = 0)
    {
        // If they already exist, get rid of them
        if (firePositions.Length <= 0 || firePositions == null)
        {
            Debug.Log("[ProjectileSource]: Builing cone based on: " + numPositions + " projectile sources");
            // build the shoot positions based off of forward vector
            firePositions = new Transform[numPositions];    // Get a new array amounting to the number of projectiles that should be fired
            CreateProjectilePositions(numPositions);
        }
    }

    public void CreateProjectilePositions(int numPositions)
    {
        float angleBetween = angleStep / numPositions;

        // Create new transforms and position them to the 
        for (int i = 0; i < numPositions; i++)
        {
            GameObject obj = new GameObject();
            obj.name = "firePosition" + (i + 1);        
            float angle = angleBetween * i;     
            if (i % 2 == 0)
            {
                // for any even projectile numbers, need to offset the source angle such that the cone lines up to sides instead of source maintaining direct forward vector
                // any odd, we can maintain the direct forward vector       
                angle *= -1;
            }

            obj.transform.parent = fireLocation;
            firePositions[i] = obj.transform;       

            // apply movement and offsets to the created object
            Quaternion rot = Quaternion.Euler(new Vector3(0, angle, 0));        
            firePositions[i].rotation = rot;
            firePositions[i].position = fireLocation.position + (fireLocation.transform.forward * offset);
        }
    }
    public GameObject GetPooledObject(ObjectPoolManager.PoolTypes type, int posIndex)
    {
        GameObject projectile = ObjectPoolManager.Instance.GetPooledObject(type);
        if (projectile != null)
        {
            Transform pos = firePositions[posIndex];
            projectile.transform.position = pos.position;
            //projectile.transform.rotation = pos.rotation;
            if (!(projectile.activeInHierarchy))
            {
                projectile.SetActive(true);
            }
        }
        return projectile;
    }

    public Projectile GetPooledProjectile(ObjectPoolManager.PoolTypes type, int posIndex)
    {
        GameObject obj = ObjectPoolManager.Instance.GetPooledObject(type);
        Projectile proj = null;
        if (obj != null)
        {
            Transform pos = firePositions[posIndex];
            obj.transform.position = pos.position;
            obj.transform.rotation = pos.rotation;
            proj = obj.GetComponent<Projectile>();
            proj.Source = transform.gameObject;
            proj.InitHitbox();
            if (!(obj.activeInHierarchy))
            {
                obj.SetActive(true);
            }
        }

        return proj;
    }

    public void OnDrawGizmosSelected()
    {
        if (firePositions != null && firePositions.Length > 0)
        {
            foreach (Transform t in firePositions)
            {
                Debug.DrawRay(t.position, t.forward * 1.0f, Color.blue);
            }
        }
    }
}
