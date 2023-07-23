using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestState : IState
{
    private HunterAgent _agent;
    private StateMachine _fsm;
    public RestState(HunterAgent hunter,StateMachine fsm)
    {
        _agent = hunter;
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }


    public void OnUpdate()
    {
        if (_agent.currentEnergy > _agent.maxEnergy)
        {
            _agent.currentEnergy = _agent.maxEnergy;
        }
        if (_agent.currentEnergy < _agent.maxEnergy)
        {
            _agent.currentEnergy += _agent.energyRecoveryRate*Time.deltaTime;
        }
        else
        {
            CheckForTargets();
        }
    }
    public void OnExit()
    {
    }
    private void CheckForTargets()
    {
        if (AutoAgentManager.instance.allAgents != null && AutoAgentManager.instance.allAgents.Count > 0)
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
            if (_agent.currentEnergy >= _agent.chaseEnergyCost)
            {
                _fsm.ChangeState(AgentStates.Chase);
            }
            else
            {
                if (_agent.currentEnergy >= _agent.patrolEnergyCost)
                {
                    _fsm.ChangeState(AgentStates.Patrol);
                }
            }
        }
        else
        {
            if (_agent.currentEnergy >= _agent.patrolEnergyCost)
            {
                _fsm.ChangeState(AgentStates.Patrol);
            }
        }
    }
}
