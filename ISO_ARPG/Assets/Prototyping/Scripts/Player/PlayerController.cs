using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool DebugLogs = false;

    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private float rotSpeed = 10.0f;

    [SerializeField]
    private GameObject damageCube = null;

    [SerializeField]
    private float attackDelay = 0.5f;

    bool attacking = false;
    bool canMove = true;

    private Vector3 moveTarget;

    public List<Skill> skills = new List<Skill>();

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (skills.Count > 0)
            {
                skills[0].UseSkill();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (DebugLogs)
                Debug.Log("Left Clicked!");

            if (damageCube != null)
            {
                StopAllCoroutines();
                StartCoroutine(Attacking(attackDelay));
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetMoveTarget();
        }

        if (moveTarget != Vector3.zero)
        {
            if (canMove)
            { 
                UpdateRotation();
                UpdatePosition();
            }
        }
    }

    IEnumerator Attacking(float delay)
    {
        attacking = true;

        if (attacking)
        { 
            damageCube.SetActive(attacking);
            canMove = false;
        }
        yield return new WaitForSeconds(delay);
        attacking = false;
        canMove = true;
        damageCube.SetActive(false);
    }

    private void UpdateRotation()
    { 
        // get the direction
        Vector3 dir = moveTarget - transform.position;
        dir.Normalize();
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
    }
    private void UpdatePosition()
    {
        moveTarget.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);

        // if the destination is reached.
        if (transform.position == moveTarget)
        {
            moveTarget = Vector3.zero;
        }
    }

    // call this whenever the mouse is clicked
    void GetMoveTarget()
    {
        RaycastHit hit;

        // for the raycast call, experiment with the layer masking to only interact with the physics on the relevant layer
        // need to check if mouse is on valid location (ground layer)
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
        {
            if (DebugLogs)
                Debug.Log("Hit: " + LayerMask.LayerToName(hit.transform.gameObject.layer));
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                moveTarget = hit.point;
                // initialize a game object or particle using this move target to show the player where there click will move them (after clicked, little indicator to show)

                if (DebugLogs)
                    Debug.Log("Set move point to: " + moveTarget);
            }
            else
            { 
                if (DebugLogs)
                    Debug.Log("Not on the ground...");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }

}
