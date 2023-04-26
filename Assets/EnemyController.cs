using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;
    void Start()
    {
        
    }

    void Update()
    {
        agent.SetDestination(player.position);
        
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
