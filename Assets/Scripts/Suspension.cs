using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SocialPlatforms;
public class Suspension : MonoBehaviour
{

    public Rigidbody rigidBody;
    public float damping = 10f;
    public float springCoefficient = 10f;
    public float mu = 1;
    public float rollResistance = 0.1f;

    public float timer = 0f;
    public float flipTime = 5f;
    public float length = 10;
    public Vector3[] wheelPositions;
    private float[] lengths;

    public bool isGrounded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lengths = new float[wheelPositions.Length];
    }

    // Update is called once per frame
    void Update()
    {
        float normalForce = rigidBody.mass * 9.81f;
        isGrounded = false;
        for (int i = 0; i < wheelPositions.Length; i++)
        {
            Vector3 worldPosition = rigidBody.transform.TransformPoint(wheelPositions[i]);
            RaycastHit hit;
            Debug.DrawRay(worldPosition, -rigidBody.transform.up * length);

            Vector3 goal = worldPosition - rigidBody.transform.up * length;


            bool detectGround = Physics.Raycast(worldPosition, -rigidBody.transform.up, out hit, length);


            if (detectGround)
            {
                Debug.DrawRay(hit.point, Vector3.up * 0.2f, Color.red);

                Vector3 springDirection = (goal - worldPosition).normalized;
                float currentLength = (worldPosition - hit.point).magnitude;
                float velocityAlongSpring = currentLength - lengths[i];
                float displacement = (goal - hit.point).magnitude;
                float springForceStrength = springCoefficient * displacement - damping * velocityAlongSpring;

                normalForce += springForceStrength;
                //wheels[i].GetComponent<Rigidbody>().AddForceAtPosition(-springDirection * force * Time.deltaTime, worldPosition);
                rigidBody.AddForceAtPosition(-springDirection * springForceStrength * Time.deltaTime, worldPosition);
            }


            if (detectGround)
            {
                isGrounded = true;
                lengths[i] = (hit.point - worldPosition).magnitude;
            }
            else
            {
                lengths[i] = length;
            }


            //wheels[i].transform.localPosition = new Vector3(wheelPositions[i].x, wheels[i].transform.localPosition.y, wheelPositions[i].z);
        }
        if (isGrounded)
        {
            timer = 0f;
            float frictionForce = mu * normalForce;
            Vector3 localVelocity1 = transform.InverseTransformDirection(rigidBody.linearVelocity);
            Vector3 localVelocity2 = transform.InverseTransformDirection(rigidBody.linearVelocity);
            localVelocity1.z = 0;
            localVelocity1.y = 0;

            localVelocity2.x = 0;
            localVelocity2.y = 0;

            Vector3 lateralVelocity = transform.TransformDirection(localVelocity1);
            Vector3 forwardVelocity = transform.TransformDirection(localVelocity2);

            Vector3 localAngularVelocity = transform.InverseTransformDirection(rigidBody.angularVelocity);
            localAngularVelocity.x = 0;
            localAngularVelocity.z = 0;

            Vector3 lateralAngularVelocity = transform.TransformDirection(localAngularVelocity);

            rigidBody.AddForce(-lateralVelocity * frictionForce * Time.deltaTime - forwardVelocity * rigidBody.mass * 9.81f * rollResistance * Time.deltaTime);
            rigidBody.AddTorque(-lateralAngularVelocity * rigidBody.mass * 9.81f * rollResistance * Time.deltaTime);
        } else
        {
            timer += Time.deltaTime;
            if (timer >= flipTime)
            {
                timer = 0f;
                LeanTween.rotate(gameObject, Vector3.zero, 1f);
            }
        }
    }

}
