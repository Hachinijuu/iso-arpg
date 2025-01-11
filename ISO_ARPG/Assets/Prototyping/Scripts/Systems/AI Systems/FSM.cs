using UnityEngine;

public class FSM : MonoBehaviour
{
    //Player Transform
    protected Transform playerTransform;

    // Destination Position
    protected Vector3 destPos;

    //Arrow Firing Rate
    protected float arrowShootRate;
    protected float arrowElapsedTime;

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }



    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }
    private void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}