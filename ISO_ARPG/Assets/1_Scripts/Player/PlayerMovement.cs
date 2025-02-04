using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    #region VARIABLES
    // Read controls via external game manager script -- cache locally?
    public enum MoveInput { CLICK, DIRECTIONAL }
    public MoveInput moveType;
    // CLICK is mouse click to move / hold to move
    // DIRECTIONAL is WASD / Joystick

    [SerializeField] LayerMask moveMask;

    // Classes
    NavMeshAgent agent;
    PlayerInput input;
    Animator anim;

    // Public Accessors
    public Vector3 MoveTarget { get { return moveTarget; } }
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool CanRotate { get { return canRotate; } set { canRotate = value; } }
    public float Speed { get { return Speed; } set { speed = value; } }

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
        agent = GetComponent<NavMeshAgent>();
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        // map inputs
        MapMovementActions();
    }

    private void Start()
    {
        //GameManager.Instance.onMoveChanged += UpdateMoveType;

        canMove = false;
        canRotate = false;
    }

    private void OnDestroy()
    {
        UnmapMovementActions();
    }

    public void Reset()
    {
        //transform.position = transform.position;
        moveTarget = transform.position;
        agent.destination = moveTarget;
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
        if (canMove)                        // If the player can move
        {
            agent.destination = moveTarget;       // Move to the target
            speed = agent.velocity.normalized.magnitude;
        }
        else
        {
            speed = 0.0f;
        }

        // Regardless of movement handle the character's rotation
        if (canRotate)
            HandleRotation();

        // Animation handling
        if (UseAnimations)
        {
            anim.SetFloat("Speed", speed);
        }

    }
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
        // TODO: LIMIT RAY DISTANCE SO MORE COST EFFECTIVE
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, moveMask))
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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, moveMask))
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
    #endregion
}
