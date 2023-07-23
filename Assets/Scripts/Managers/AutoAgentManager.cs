using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAgentManager : MonoBehaviour
{
    public List<AutoAgent> allAgents = new List<AutoAgent>();
    public static AutoAgentManager instance;

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
    public void AddAutoAgent(AutoAgent autoAgent)
    {
        if (!allAgents.Contains(autoAgent))
        {
            allAgents.Add(autoAgent);
        }
    }
    public void QuitAutoAgent(AutoAgent autoAgent)
    {
        if (allAgents.Contains(autoAgent))
        {
            allAgents.Remove(autoAgent);
        }
    }
}
