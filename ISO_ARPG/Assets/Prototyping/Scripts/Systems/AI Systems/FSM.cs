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

    protected float updateTime;
    protected float physicsTime;

    public float UpdateInterval { get { return updateInterval; } }
    protected float updateInterval;

    public float PhysicsInterval { get { return physicsInterval; } }
    protected float physicsInterval;

    protected virtual void Initialize()
    {
        updateInterval = 0.5f;
        physicsInterval = 2.0f;
    }

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
        updateTime += Time.deltaTime;
        if (updateTime >= updateInterval)
        {
            FSMUpdate();
            updateTime = 0;
        }
    }
    private void FixedUpdate()
    {
        physicsTime += Time.fixedDeltaTime;
        if (physicsTime >= physicsInterval)
        {
            FSMFixedUpdate();
            physicsTime = 0;
        }
    }
}