using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerMovement : MonoBehaviour
{
    enum MoveInput { CLICK, DIRECTIONAL }
    [SerializeField] MoveInput moveType;
    // Click to move is mouse click to move
    // Directional is WASD / Joystick

    // Classes
    NavMeshAgent agent;
    PlayerInput input;

    // Public Accessors
    public bool CanMove { get { return canMove; } set { canMove = value; } }

    // Variables
    Vector3 moveTarget;
    private bool canMove;
    private bool moveHeld;

    private void Awake()
    {
        // get the references on THIS object
        agent = GetComponent<NavMeshAgent>();
        input = GetComponent<PlayerInput>();

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
            case MoveInput.CLICK:
                HandleClickToMove();
                break;
            case MoveInput.DIRECTIONAL:
                HandleDirectionalMove();
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
            agent.SetDestination(moveTarget);       // Move to the target
        }
    }
}
