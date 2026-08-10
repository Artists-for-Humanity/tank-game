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
    public HealthComponent healthComponent;

    private float rotatePercent = 0f;

    private GameObject mainRotorAxis;
    private GameObject extraRotorAxis;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainRotorAxis = transform.Find("MainRotorAxis").gameObject;
        extraRotorAxis = transform.Find("ExtraRotorAxis").gameObject;

        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();

        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.updateRotation = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        
        follow = GameObject.FindGameObjectWithTag("Player");
        healthComponent.onDied += () =>
        {
            Destroy(gameObject);

            LevelManager.Instance.AddExperience(500f);
        };
    }

    void Update()
    {
        rotatePercent += Time.deltaTime;
        rotatePercent %= 1f;
        print(rotatePercent.ToString());

        mainRotorAxis.transform.localEulerAngles = new Vector3(0, rotatePercent * 360f, 0);
        extraRotorAxis.transform.localEulerAngles = new Vector3(rotatePercent * 360f, 0, 0);

        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        agent.nextPosition = transform.position;
        agent.SetDestination(follow.transform.position);

        Vector3 vertical = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 50, ~(1 << gameObject.layer)))
        {
            float distance = hit.distance;

            vertical = Vector3.up * distance * 0.05f * rigidBody.mass * Physics.gravity.magnitude;
        }


        rigidBody.linearVelocity = agent.desiredVelocity * 5 + vertical;

        RotateBase(agent.desiredVelocity.normalized);
    }

    void RotateBase(Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();

        //Quaternion baseRotationTarget = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);
        
        
        Quaternion diff = Quaternion.LookRotation(direction) * Quaternion.Inverse(transform.rotation);

        diff.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
        {
            angle -= 360f;
        }

        float angleRad = angle * Mathf.Deg2Rad;

        Vector3 goalAngularVelocity = axis.normalized * angleRad * 5;
        rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, goalAngularVelocity, Time.fixedDeltaTime * 5);
    }
}
