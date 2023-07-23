using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
  public void Eat()
    {
        FoodSpawn.instance.QuitFood(this);
        Destroy(gameObject);
    }
}
