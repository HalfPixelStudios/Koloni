using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GlobalContainer;

public class PlayerAI : MonoBehaviour
{
    private Vector3 target;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(checkFloorClick(out hit)) agent.destination = hit.point;
        }
        



    }
}
