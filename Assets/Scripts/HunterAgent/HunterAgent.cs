using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentStates
{
    Rest,
    Patrol,
    Chase
}
public class HunterAgent : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public float speed;
    public float currentEnergy;
    public float maxEnergy;
    public float energyRecoveryRate;
    public float patrolEnergyCost;
    public float chaseEnergyCost;
    public float pursuitRange;
    public float maxForcePursuitOnly;
    public float maxForceDivisorPursuitOnly;
    public Transform target;
    public float _distanceToEat;
    private StateMachine _fsm;

    [SerializeField]
    private AgentStates startState;
    void Start()
    {
        _fsm = new StateMachine();

        _fsm.AddState(AgentStates.Rest, new RestState(this,_fsm));
        _fsm.AddState(AgentStates.Patrol, new PatrolState(this, _fsm));
        _fsm.AddState(AgentStates.Chase, new ChaseState(this, _fsm));
        _fsm.ChangeState(startState);
    }
    void Update()
    {
        _fsm.Update();
    }
    [SerializeField]
    private float _minXBoundValue;
    [SerializeField]
    private float _maxXBoundValue;
    [SerializeField]
    private float _minYBoundValue;
    [SerializeField]
    private float _maxYBoundValue;

    void CheckBounds()
    {
        if (transform.position.x > _maxXBoundValue)
        {
            transform.position = new Vector3(_minXBoundValue, transform.position.y, 0);
        }
        if (transform.position.x < _minXBoundValue)
        {
            transform.position = new Vector3(_maxXBoundValue, transform.position.y, 0);
        }
        if (transform.position.y > _maxYBoundValue)
        {
            transform.position = new Vector3(transform.position.x, _minYBoundValue, 0);
        }
        if (transform.position.y < _minYBoundValue)
        {
            transform.position = new Vector3(transform.position.x, _maxYBoundValue, 0);
        }
    }
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pursuitRange);
        }
    }

}
