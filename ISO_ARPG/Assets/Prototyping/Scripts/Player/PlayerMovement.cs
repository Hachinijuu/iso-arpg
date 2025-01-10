using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    // Read controls via external game manager script -- cache locally?
    enum ControlType { MOUSE_KEYBOARD, CONTROLLER }
    enum MoveInput { CLICK, DIRECTIONAL }
    [SerializeField] MoveInput moveType;
    // CLICK is mouse click to move / hold to move
    // DIRECTIONAL is WASD / Joystick

    // Classes
    NavMeshAgent agent;
    PlayerInput input;
    Animator anim;

    // Public Accessors
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool CanRotate { get { return canRotate; } set { canRotate = value; } }
    public float Speed { get { return Speed; } set { speed = value; } }

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

    private void Awake()
    {
        // get the references on THIS object
        agent = GetComponent<NavMeshAgent>();
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        // map inputs
        MapMovementActions();
        canMove = true;
    }

    private void OnDestroy()
    {
        UnmapMovementActions();
    }
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
        //input.actions["DirectionalMove"].started -= context => { };
        //input.actions["DirectionalMove"].canceled -= context => { };
        input.actions["MoveConfirmed"].started -= context => { };
        input.actions["MoveConfirmed"].canceled -= context => { };
        input.actions["StopMove"].started -= context => { };
        input.actions["StopMove"].canceled -= context => { };
    }

    private void GetMoveTarget()
    {
        // Get the target differently depending on click to move vs directional input

        switch (moveType)
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

    void HandleClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            moveTarget = hit.point;
        }
    }

    void HandleDirectionalMove()
    {
        Vector3 currPoint = transform.position;
        Vector2 moveAxis = input.actions["DirectionalMove"].ReadValue<Vector2>();
        float h = moveAxis.x;   // horizontal movement
        float v = moveAxis.y;   // vertical movement

        // for directional move, need to read the input axis and output a destination position based on them.
        // create a projection point based on the direction
        moveTarget = new Vector3(currPoint.x + h, currPoint.y, currPoint.z + v);
    }

    void HandleRotation()
    {
        // For mouse keyboard controls, drive rotation by mouse

        // Get look direction relative to the mouse position in the world
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            Vector3 mousePoint = hit.point;

            lookDirection = mousePoint - transform.position;
            lookDirection.Normalize(); // Normalize the look direction
        }

        // Apply the look
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void HandleStops(bool stop)
    {
        agent.isStopped = stop;             // stop the agent in place
        moveTarget = transform.position;    // reset target to current point
    }

    // Update is called once per frame
    void Update()
    {
        if (moveType == MoveInput.DIRECTIONAL)
        {
            GetMoveTarget();
        }
        else if (moveType == MoveInput.CLICK)
        {
            if (moveHeld)                   // If the move key is pressed
            {
                GetMoveTarget();            // Get where to move
            }
        }
        if (canMove)                     // If the player can move
        {
            agent.destination = moveTarget;       // Move to the target
            speed = agent.velocity.normalized.magnitude;
        }

        // Regardless of movement handle the character's rotation
        if (canRotate)
            HandleRotation();

        // Animation handling

        //Vector3 dir = moveTarget - transform.position;
        //dir.Normalize(); // Normalize the look direction
        //Debug.Log(dir.magnitude);

        //Debug.Log(agent.velocity.normalized.magnitude);

        if (UseAnimations)
        {
            anim.SetFloat("Speed", speed);
        }

        //if (useAnimations)
        //{
        //    float curSpeed = agent.velocity.normalized.magnitude;
        //    if (curSpeed > 0.75)
        //    {
        //        // Clamp the value dead on
        //        curSpeed = 1;
        //    }
        //    //Debug.Log(curSpeed);
        //    anim.SetFloat("Speed", curSpeed);
        //
        //}

        // Need to get the magnitude in the movement direction

    }
}
