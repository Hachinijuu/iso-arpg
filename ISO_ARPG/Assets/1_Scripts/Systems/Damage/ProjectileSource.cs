using UnityEngine;

public class ProjectileSource : MonoBehaviour
{
    [SerializeField] Transform[] firePositions;
    [SerializeField] float offset = 2.0f;
    [SerializeField] float angleStep = 180.0f;

    // this should listen to stats, and init fire positions whenever the value of the projectiles changes
    // this will maintain updated projectile counts throughout the game, via gear and such

    // if no stats exists, the projectiles source will default to a predefined count outlined by the script itself

    public void Start()
    {
        // Check if this script is attached to a player
        if (gameObject.tag == "Player")
        {
            // If it is, we want the fire positions to LISTEN to the number of projectiles, if the number of projectiles changes, then the number of projectiles sent out changes
            PlayerStats stats = GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Projectiles.Changed += context => { InitFirePositions((int)context); };
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
    public void InitFirePositions(int numPositions = 1)
    {
        if (firePositions.Length <= 0 || firePositions == null)
        {
            // build the shoot positions based off of forward vector
            firePositions = new Transform[numPositions];    // Get a new array amounting to the number of projectiles that should be fired

            float angleBetween = angleStep / numPositions;

            // Create new transforms and position them to the 
            for (int i = 0; i < numPositions; i++)
            {
                GameObject obj = new GameObject();
                obj.name = "firePosition" + (i + 1);
                obj.transform.parent = transform;
                firePositions[i] = obj.transform;

                //Quaternion rot = firePositions[i].rotation;
                float angle = angleBetween;

                if (i % 2 == 0)
                {
                    // for any even projectile numbers, need to offset the source angle such that the cone lines up to sides instead of source maintaining direct forward vector
                    // any odd, we can maintain the direct forward vector

                    angle *= -1;
                }
                Quaternion rot = Quaternion.Euler(new Vector3(0, angle, 0));

                //Quaternion.Eul
                transform.rotation = rot;
                Debug.Log(obj.name + " with " + obj.transform.rotation + " angle: " + angle);

                firePositions[i].position += (obj.transform.forward * offset);
                // take 180 frontal cone, divide it by the number of projectiles that should exist to get the angles between each source


                // rotate based on the number of projectiles that exists
                // project a forward direction to get a cone


                //Vector3.RotateTowards(transform.position, firePositions[i].position, )
            }

            //foreach (Transform t in firePositions)
            //{
            //    //Transform temp = Instantiate(Transform )
            //    t = transform;  // assign the value to whatever the source is on
            //    //t.position = transform.position + (transform.forward * 1.0f);
            //
            //}

            // setting up the fire positions
            //Vector3.Dot(transform.forward, firePositions[0].position);
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
