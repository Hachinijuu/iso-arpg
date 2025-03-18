using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    #region VARIABLES
    // Read controls via external game manager script -- cache locally?
    public enum MoveInput { CLICK, DIRECTIONAL }
    public MoveInput type;
    //public MoveInput moveType;
    // CLICK is mouse click to move / hold to move
    // DIRECTIONAL is WASD / Joystick

    [SerializeField] LayerMask moveMask;
    [SerializeField] int raycastLength = 50;
    [SerializeField] GameObject characterBody;  // This refers to the bones in this case
    [SerializeField] Rigidbody body;

    // Classes
    PlayerInput input;
    PlayerStats stats;
    NavMeshAgent agent;
    [SerializeField] Animator anim;

    // Public Accessors
    public Vector3 MoveTarget { get { return moveTarget; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool CanRotate { get { return canRotate; } set { canRotate = value; } }
    public float Speed { get { return speed; } set { speed = value; } }

    // Expose this property to allow movement to be driven externally (by mouse holding)
    public bool MoveHeld { get { return moveHeld; } set { moveHeld = value; } }
    public bool UseAnimations { get { return useAnimations; } set { useAnimations = value; } }
    // Variables
    Vector3 moveTarget;
    Vector3 lookDirection;
    private bool canMove = true;
    private bool canRotate = true;
    private bool moveHeld = false;
    private bool useAnimations = true;

    [SerializeField] int rotationSpeed = 20;
    private float speed;
    #endregion
    #region UNITY FUNCTIONS
    private void Awake()
    {
        // get the references on THIS object
        input = GetComponent<PlayerInput>();
        stats = GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();

        //agent.updatePosition = false;
        //agent.updateRotation = false;
        // map inputs
        //MapMovementActions();
    }
    private void OnEnable() 
    {
        MapMovementActions();
    }

    private void OnDisable() 
    {
        UnmapMovementActions();
    }
    public void Respawn()
    {
        transform.position = transform.position;
        moveTarget = transform.position;
        //agent.destination = moveTarget;
        //Debug.Log(transform.position);

        canMove = true;
        canRotate = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.currGameState != GameManager.GameState.PLAYING)    // is a state check efficient
            return;

        if (type == MoveInput.DIRECTIONAL)
        {
            GetMoveTarget();
        }
        else if (type == MoveInput.CLICK)
        {
            if (moveHeld)                   // If the move key is pressed
            {
                GetMoveTarget();            // Get where to move
            }
        }
        if (canMove)                        // If the player can move
        {
            // Ideally player makes use of NavMesh - temporary movement solution while AI navigation gets looked at
            HandleMovement();

            // When the player is moving, fire off an event for the AIManager to listen to
            // This will allow the AIManager to only update the flow field when the player is presumeably in a different cell
            // On the player end, this will evaluate the movement
            // On the AIManager end, determine the distance between the current cell, and see if it has exceeded the size of a given cell
            
            //agent.destination = moveTarget;       // Move to the target
            //speed = agent.velocity.normalized.magnitude;
        }
        else
        {
            speed = 0.0f;
        }

        // Regardless of movement handle the character's rotation
        if (canRotate)
        {
            if (moveHeld || speed == 0.0f) // If move is held or speed is 0, handle it
                HandleRotation();
        }

        // Animation handling
        if (UseAnimations)
        {
            anim.SetFloat("Speed", speed);
        }

    }
    #endregion
    #region EVENTS
    public class MoveArgs
    {
        bool moving;
        float speed;
        
        public MoveArgs(bool on, float speed)
        {
            moving = on;
            this.speed = speed;
        }
    }
    public delegate void Moving(MoveArgs e);
    public event Moving onMove;
    protected void FireMoving(MoveArgs e) { if (onMove != null) { onMove(e); } } 
    #endregion
    #region ACTION MAPPING
    private void MapMovementActions()
    {
        // map the actions based on input scheme, reuse this call when the options are switched (map to correct scheme)

        // can do remapping based on schemes, but input detection is based on type flag (changes automatically)
        // therefore in settings change the flag - would automatically change the input detection
        input.actions["MoveConfirmed"].started += context =>
        {
            moveHeld = true;
        };
        input.actions["MoveConfirmed"].canceled += context =>
        {
            moveHeld = false;
        };
        input.actions["StopMove"].started += context =>
        {
            canMove = false;
            HandleStops(!canMove);
        };
        input.actions["StopMove"].canceled += context =>
        {
            canMove = true;
            HandleStops(!canMove);
        };
    }

    private void UnmapMovementActions()
    {
        // unmap all regardless of which input is used

        input.actions["MoveConfirmed"].started -= context => { };
        input.actions["MoveConfirmed"].canceled -= context => { };
        input.actions["StopMove"].started -= context => { };
        input.actions["StopMove"].canceled -= context => { };
    }
    #endregion
    #region FUNCTIONALITY
    private void GetMoveTarget()
    {
        // Get the target differently depending on click to move vs directional input

        switch (type)
        {
            case MoveInput.DIRECTIONAL:
                HandleDirectionalMove();
                break;
            case MoveInput.CLICK:
            default:
                HandleClickToMove();
                break;
        }
    }

    public float clickBuffer = 0.05f;
    void HandleClickToMove()
    {
        RaycastHit hit;
        // TODO: LIMIT RAY DISTANCE SO MORE COST EFFECTIVE
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastLength, moveMask))
        {
            // If the movetarget is TOO close the player, disregard it
            if (Vector3.Distance(hit.point, transform.position) > clickBuffer)
            {
                moveTarget = hit.point;
            }
        }
    }

    void HandleDirectionalMove()
    {
        Vector3 currPoint = transform.position;
        Vector2 moveAxis = input.actions["DirectionalMove"].ReadValue<Vector2>();
        // Offset the value by the speed
        float h = moveAxis.x; //* agent.speed;   // horizontal movement
        float v = moveAxis.y; //* agent.speed;   // vertical movement
        // This needs to be relative to the camera positioning, such that the camera can be setup and the directional movement will work properly given all camera directions


        // for directional move, need to read the input axis and output a destination position based on them.
        // create a projection point based on the direction
        moveTarget = new Vector3(currPoint.x + h, currPoint.y, currPoint.z + v);
    }

    void HandleRotation()
    {
        // For mouse keyboard controls, drive rotation by mouse

        // Get look direction relative to the mouse position in the world
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, raycastLength, moveMask))
        {
            Vector3 mousePoint = hit.point;

            lookDirection = mousePoint - transform.position;
            lookDirection.Normalize(); // Normalize the look direction
            //lookDirection.y = transform.position.y + 1; // Offset from the ground
        }

        // Apply the look
        //lookDirection.x = -90;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // Current rotation (full transform)
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //targetRotation.x = -90f;
        //Quaternion temp = characterBody.transform.rotation;
        //characterBody.transform.localRotation = Quaternion.Slerp(characterBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //temp.rotation.y = Quaternion.LookRotation(lookDirection, Vector3.up).y;

        // ALTERNATE ROTATION -- rig body (requires y up mesh, so z is proper) -- would maintain relative world values for slot system
        characterBody.transform.rotation = Quaternion.Slerp(characterBody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.MoveTowards(transform.position, moveTarget, stats.MoveSpeed.Value * Time.deltaTime);
        //movement.y = 0;

        //body.MovePosition(movement);
        transform.position = movement;
        //agent.destination = moveTarget;
        speed = (transform.position - moveTarget).magnitude;    // Magnitude of the direction
        //Debug.Log(speed);
        //Debug.Log(agent.desiredVelocity);
        //speed = body.velocity.magnitude;//(transform.position - moveTarget).magnitude;    // Magnitude of the direction
        if (speed <= 0.1f)
            speed = 0;
        if (speed > 0)
        {
            FireMoving(new MoveArgs(true, speed));
        }
        // Debug.Log(speed);
    }
    public void HandleStops(bool stop)
    {
        //agent.isStopped = stop;             // stop the agent in place
        canMove = stop;
        moveTarget = transform.position;    // reset target to current point
    }
    #endregion
}
