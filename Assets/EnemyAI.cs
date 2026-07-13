using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public NavMeshAgent agent;
    public GameObject follow;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(follow.transform.position);
    }
}