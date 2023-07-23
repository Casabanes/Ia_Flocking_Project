using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private HunterAgent _agent;
    private StateMachine _fsm;
    private AutoAgent _actualTarget;
    private Vector3 _velocity;
    public ChaseState(HunterAgent hunter, StateMachine fsm)
    {
        _agent = hunter;
        _fsm = fsm;
    }
    public void OnEnter()
    {
        if (_agent.target == null)
        {
            CheckForTargets();
        }
    }
    public void OnUpdate()
    {
        Pursuit();
    }
    public void OnExit()
    {
        _agent.target = null;
        _actualTarget = null;
    }
    private void Pursuit()
    {
        if (_agent.target == null)
        {
            CheckForTargets();
        }
        if (_agent.currentEnergy < _agent.chaseEnergyCost)
        {
            CheckEnergyForOtherStates();
            return;
        }
        if (_actualTarget == null)
        {
            if (_agent.target != null)
            {
                _actualTarget = _agent.target.GetComponent<AutoAgent>();
            }

        }
        if(Vector3.Distance(_actualTarget.transform.position, _agent.transform.position) > _agent.pursuitRange)
        {
            _agent.target = null;
            _actualTarget = null;
            CheckForTargets();
            return;
        }
        Vector3 futurePosition = _agent.target.transform.position + _actualTarget.GetVelocity();
        Vector3 desired = (futurePosition - _agent.transform.position);
        if (Vector3.Distance(_agent.target.position, _agent.transform.position)
            < _actualTarget.GetVelocity().magnitude)
        {
            desired = (_agent.target.transform.position - _agent.transform.position);
        }

        desired = desired.normalized * _agent.speed;
        desired.z = 0;
        Vector3 steering = Vector3.ClampMagnitude
            (desired - _velocity, _agent.maxForcePursuitOnly / _agent.maxForceDivisorPursuitOnly);
        ApplyForce(steering);
        _agent.transform.position += _velocity * Time.deltaTime;
        _agent.transform.forward = _velocity;
        _agent.currentEnergy -= _agent.chaseEnergyCost * Time.deltaTime;
        if (Vector3.Distance(_actualTarget.transform.position, _agent.transform.position) < _agent._distanceToEat)
        {
            _actualTarget.BeEaten();
            _actualTarget = null;
            _agent.target = null;
            CheckForTargets();
        }
    }
    private void ApplyForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity+ force,_agent.speed);
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
                        _actualTarget = autoAgent;
                    }
                }
            }
        }
        if (_agent.target == null)
        {
            CheckEnergyForOtherStates();
        }
    }
    private void CheckEnergyForOtherStates()
    {
        if (_agent.currentEnergy >= _agent.patrolEnergyCost)
        {
            _fsm.ChangeState(AgentStates.Patrol);
        }
        else
        {
            _fsm.ChangeState(AgentStates.Rest);
        }
    }
}
