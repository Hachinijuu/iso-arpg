using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    // THIS CLASS HAS A NAVMESH DEPENDENCY, IT IS NOT IMPLEMENTED YET...
    
    /*
    [SerializeField]
    NavMeshAgent agent;
    public GameObject target = null;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            target = GameObject.FindWithTag("Player");
        agent.SetDestination(target.transform.position);
    }

    private void OnEnable()
    {
        if (target != null)
            agent.SetDestination(target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //agent.Move(agent.nextPosition);

        //if (!agent.hasPath)
        //{ 
        //}

        //float distance = Vector3.Distance(agent.transform.position, target.transform.position);
        agent.SetDestination(target.transform.position);    // CONSTANTLY UPDATING WHENEVER THE PLAYER MOVES

        //if (!agent.hasPath && !agent.isStopped) // if a new path needs to be recalculated
        //{
        //    if (agent.SetDestination(target.transform.position))    // recalculate a path to the target
        //    {
        //        Debug.Log("Moving to the target");
        //    }   
        //}
    }

    */
}
