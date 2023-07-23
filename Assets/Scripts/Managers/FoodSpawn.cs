using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour
{
    [SerializeField]
    private Food _food;
    [SerializeField]
    private float _minXPosition;
    [SerializeField]
    private float _maxXPosition;
    [SerializeField]
    private float _minYPosition;
    [SerializeField]
    private float _maxYPosition;
    [SerializeField]
    private float _spawnRate;
    public List<Food> allFoodInScene=new List<Food>();
    public static FoodSpawn instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(InstantiateFood());
    }
    private IEnumerator InstantiateFood()
    {
        yield return new WaitForSeconds(_spawnRate);
        Food instance= Instantiate(_food, new Vector3(Random.Range(_minXPosition, _maxXPosition)
            ,Random.Range(_minYPosition, _maxYPosition), 0) ,_food.transform.rotation);
        allFoodInScene.Add(instance);
        StartCoroutine(InstantiateFood());
    }
    public void QuitFood(Food food)
    {
        if (allFoodInScene.Contains(food))
        {
            allFoodInScene.Remove(food);
        }
    }
}
