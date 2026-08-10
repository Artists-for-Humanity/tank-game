using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class HelicopterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject follow;
    private Rigidbody rigidBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();

        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.updateRotation = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        agent.nextPosition = transform.position;
        agent.SetDestination(follow.transform.position);

        Vector3 vertical = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 50, 1 << gameObject.layer))
        {
            float distance = hit.distance;

            vertical = Vector3.up * distance * rigidBody.mass * Physics.gravity.magnitude;
        }


        rigidBody.linearVelocity = agent.desiredVelocity + vertical;

    }
}
