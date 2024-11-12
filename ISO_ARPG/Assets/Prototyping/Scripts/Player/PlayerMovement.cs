using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
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
        input.actions["MoveConfirmed"].started -= context => { };
        input.actions["MoveConfirmed"].canceled -= context => { };
        input.actions["StopMove"].started -= context => { };
        input.actions["StopMove"].canceled -= context => { };
    }

    private void GetMoveTarget()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            moveTarget = hit.point;
        }
    }
    public void HandleStops(bool stop)
    {
        agent.isStopped = stop;             // stop the agent in place
        moveTarget = transform.position;    // reset target to current point
    }

    // Update is called once per frame
    void Update()
    {
        if (moveHeld)                   // If the move key is pressed
        {
            GetMoveTarget();            // Get where to move
        }
        if (canMove)                     // If the player can move
        {
            agent.SetDestination(moveTarget);       // Move to the target
        }
    }
}
