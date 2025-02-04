using UnityEngine;

public class CameraController : MonoBehaviour
{
    // THIS CLASS IS DEPRECATED
    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector3 angle;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");

        //angle.x = 0;
    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + offset;
        transform.rotation = Quaternion.Euler(angle);

    }
}
