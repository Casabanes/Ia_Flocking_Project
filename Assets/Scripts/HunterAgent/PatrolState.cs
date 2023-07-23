using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private HunterAgent _agent;
    private StateMachine _fsm;
    private int _waypointIndex = 0;
    public PatrolState(HunterAgent hunter,StateMachine fsm)
    {
        _agent = hunter;
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }

    public void OnUpdate()
    {
        Patrol();
        CheckForTargets();
    }
    public void OnExit()
    {
    }
    private void Patrol()
    {
        if (_agent.currentEnergy < _agent.patrolEnergyCost)
        {
            _fsm.ChangeState(AgentStates.Rest);
        }
        Vector3 dir = (_agent.waypoints[_waypointIndex].position - _agent.transform.position);

        if (dir.magnitude < 0.1f)
        {
            _waypointIndex++;
            if (_waypointIndex >= _agent.waypoints.Count)
            {
                _waypointIndex = 0;
            }
        }
        _agent.transform.position += dir.normalized * _agent.speed * Time.deltaTime;
        _agent.currentEnergy -= _agent.patrolEnergyCost * Time.deltaTime;
    }
    private void CheckForTargets()
    {
        if(AutoAgentManager.instance.allAgents!=null && AutoAgentManager.instance.allAgents.Count > 0)
        {
            float distance = _agent.pursuitRange;
            foreach (var autoAgent in AutoAgentManager.instance.allAgents)
            {
                float auxDistance = Vector3.Distance(autoAgent.transform.position, _agent.transform.position);
                if (_agent.pursuitRange > auxDistance)
                {
                    if (distance > auxDistance)
                    {
                        distance = auxDistance;
                        _agent.target = autoAgent.transform;
                    }
                }
            }
        }
        if (_agent.target != null)
        {
            if(_agent.currentEnergy >= _agent.chaseEnergyCost)
            {
                _fsm.ChangeState(AgentStates.Chase);
            }
            else
            {
                _fsm.ChangeState(AgentStates.Rest);
            }
        }
    }
}
