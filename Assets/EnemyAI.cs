using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public NavMeshAgent agent;
    public GameObject follow;

    public float baseRotationSpeed = 1.0f;
    public Rigidbody rigidBody;
    public float speed = 20.0f;

    private float updateInterval = 1.0f;
    private float timer = 0.0f;
    void Start()
    {
        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.updateRotation = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0.0f;
            agent.SetDestination(follow.transform.position);
        }
       agent.nextPosition = transform.position;

        bool isGrounded = GetComponent<Suspension>().isGrounded;
            
        if (isGrounded)
        {
            rigidBody.linearVelocity += agent.desiredVelocity.normalized * Time.deltaTime * speed;
        }

        //(cameraForward * moveDirection.y + cameraRight * moveDirection.x) * Time.deltaTime * 10;

        Vector3 baseDirection = agent.desiredVelocity;
        baseDirection.y = 0;
        baseDirection.Normalize();

        Debug.DrawRay(transform.position, agent.desiredVelocity, Color.blue);

        Quaternion baseRotationTarget = Quaternion.LookRotation(baseDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);

    }
}