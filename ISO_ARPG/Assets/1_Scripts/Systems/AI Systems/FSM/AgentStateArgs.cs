using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentStateArgs
{
    public EnemyControllerV2 agent;
    public PlayerController player;
    public Transform target;

    public AgentStateArgs()
    {
        agent = null;
        player = null;
        target = null;
    }
    public AgentStateArgs(EnemyControllerV2 agent)
    {
        this.agent = agent;
    }

    public AgentStateArgs(PlayerController player)
    {
        this.player = player;
    }

    public AgentStateArgs(EnemyControllerV2 agent, Transform target)
    {
        this.agent = agent;
        this.target = target;
    }
}
