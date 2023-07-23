using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAgent : MonoBehaviour
{
    [SerializeField]
    private Vector3 _velocity;
    private Transform target;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _maxForce;
    [SerializeField]
    private float separationWeight;
    [SerializeField]
    private float alignmentWeight;
    [SerializeField]
    private float cohesionWeight;
    [SerializeField]
    private Transform _hunter;
    [SerializeField]
    private bool _evading;
    private void Start()
    {
        AutoAgentManager.instance.AddAutoAgent(this);
        GoInAnyDirection();
        if (_hunter == null)
        {
            _hunter = FindObjectOfType<HunterAgent>().gameObject.transform;
        }
    }
    private void Update()
    {
        Applyforce(Separation() * separationWeight);
        if (!_isGoingForFood || !_evading)
        {
            Applyforce(Cohesion() * cohesionWeight + Alignment() * alignmentWeight);
        }
        Flee();
        CheckBounds();
        if (!_evading)
        {
            LookForFood();
            GoForFood();
        }
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    private void GoInAnyDirection()
    {
        Vector3 desired = new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), 0);
        desired = desired.normalized * _speed;
        Applyforce(desired);
    }
    private void Applyforce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity+ force, _speed);
    }
    [SerializeField]
    private float _separationRange;
    [SerializeField]
    private float _maxForceDivisor;
    private Vector3 Separation()
    {
        Vector3 desired = new Vector3();
        foreach (var autoAgent in AutoAgentManager.instance.allAgents)
        {
            if(autoAgent == this)
            {
                continue;
            }
            Vector3 distance = autoAgent.transform.position - transform.position;
            if (distance.magnitude <= _separationRange)
            {
                desired += distance;
                desired.z = 0;
            }
        }
        if (desired == Vector3.zero)
        {
            return desired;
        }
        desired *= -1;
        desired = desired.normalized * _speed;
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce / _maxForceDivisor);
        return (steering);
    }
    [SerializeField]
    private float _distanceToAlign;
    private Vector3 Alignment()
    {
        Vector3 desired = new Vector3();
        int count = 0;
        foreach (var autoAgent in AutoAgentManager.instance.allAgents)
        {
            if (autoAgent == this)
            {
                continue;
            }
            if (Vector3.Distance(autoAgent.transform.position, transform.position) <= _distanceToAlign)
            {
                desired += autoAgent._velocity;
                desired.z = 0;
                count++;
            }
        }
        if(count==0)
        {
            return desired;
        }
        return CalculateSteering((desired / count));
    }
    [SerializeField]
    private float _cohesionViewRadius;
    private Vector3 Cohesion()
    {
        Vector3 desired = new Vector3();
        int count = 0;
        foreach (var autoAgent in AutoAgentManager.instance.allAgents)
        {
            if (autoAgent == this)
            {
                continue;
            }
            if(Vector3.Distance(autoAgent.transform.position,transform.position)< _cohesionViewRadius)
            {
                desired += autoAgent.transform.position;
                desired.z=0;
                count++;
            }
        }
        if (count == 0)
        {
            return desired;
        }
        return CalculateSteering((desired / count) - transform.position);
    }
    private Vector3 CalculateSteering(Vector3 desired)
    {
        desired = desired.normalized * _speed;
        
        Vector3 steering = desired - _velocity;
        
        steering = Vector3.ClampMagnitude(steering, _maxForce/_maxForceDivisor);
        
        return steering;
    }
    private float _distanceToFood;
    [SerializeField]
    private float _visionRange;
    private bool _isGoingForFood;
    private void LookForFood()
    {

		if (FoodSpawn.instance.allFoodInScene != null) // chequeo que haya comida en la escena
			{
				if (FoodSpawn.instance.allFoodInScene.Count > 0)
				{
                float distance = _visionRange;
				foreach (var food in FoodSpawn.instance.allFoodInScene) // recorro las food para ver cual está mas cerca
				{
                    if(distance > Vector3.Distance(food.transform.position, transform.position))
                    {
						distance = Vector3.Distance(food.transform.position, transform.position); //asigno la mas corta
					    _distanceToFood = distance; // asigno el valor obtenido porque la otra funcion lo usa
						target = food.transform; // asigno target
						_isGoingForFood = true; // prendo el boleano para que entre al GoForFood
					}
				}
				}
		}
    }
    [SerializeField]
    private float _distanceToEat;
    private void GoForFood()
    {
        if (target == null)
        {
            _isGoingForFood = false;
        }
        if (!_isGoingForFood)
        {
            return;
        }
        Arrive();
        _distanceToFood = Vector3.Distance(target.transform.position, transform.position);
        if(_distanceToFood< _distanceToEat)
        {
            target.GetComponent<Food>().Eat();
            GoInAnyDirection();
            _isGoingForFood = false;
        }
    }
    [SerializeField]
    private float _arriveRadius;

    [SerializeField,Range(0,100)]
    private float _arrivePercentage;

    private void Arrive()
    {
        Vector3 desired = target.position - transform.position;
        desired.Normalize();
        float speed = _speed;
        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (distanceToTarget < _arriveRadius)
        {
            speed = _speed * (distanceToTarget / _arriveRadius);
        }
        desired *= speed;
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce / _arrivePercentage);
        Applyforce(steering);
    }
    public void BeEaten()
    {
        AutoAgentManager.instance.QuitAutoAgent(this);
        Destroy(gameObject);
    }
    private void Flee()
    {
        if (Vector3.Distance(_hunter.position, transform.position)<_visionRange)
        {
            _evading = true;
        Vector3 desired = _hunter.position - transform.position;
        desired.Normalize();
        desired *= _speed;
        desired *= -1;
        desired.z = 0;
        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, _maxForce / _maxForceDivisor);
        Applyforce(steering);
            return;
        }
        if(_evading)
        {
            _evading = false;
        }

    }
    private void OnDrawGizmos()
    {
        if (target != null)
        {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, _arriveRadius);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _distanceToEat);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _visionRange);
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
            transform.position = new Vector3(_minXBoundValue, transform.position.y,0);
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
}
