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

    public float length = 10;
    public Vector3[] wheelPositions;
    private float[] lengths; 
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lengths = new float[wheelPositions.Length];

    }

    // Update is called once per frame
    void Update()
    {
        float normalForce = rigidBody.mass * 9.81f;

        for (int i = 0; i < wheelPositions.Length; i++)
        {
            Vector3 worldPosition = rigidBody.transform.TransformPoint(wheelPositions[i]);
            RaycastHit hit;
            Debug.DrawRay(worldPosition, -rigidBody.transform.up * length);
            
            Vector3 goal = worldPosition - rigidBody.transform.up * length;


            bool detectGround = Physics.Raycast(worldPosition, -rigidBody.transform.up, out hit, length);


            if (detectGround)
            {
                Debug.DrawRay(hit.point, Vector3.up * 0.1f);
            
                Vector3 springDirection = (goal - worldPosition).normalized;
                float currentLength = (worldPosition - hit.point).magnitude;

                float velocityAlongSpring = lengths[i] - currentLength;
                
                float displacement = (goal - hit.point).magnitude;

                float springForceStrength = springCoefficient * displacement - damping * -velocityAlongSpring;

                normalForce += springForceStrength;
                //wheels[i].GetComponent<Rigidbody>().AddForceAtPosition(-springDirection * force * Time.deltaTime, worldPosition);
                rigidBody.AddForceAtPosition(-springDirection * springForceStrength * Time.deltaTime, worldPosition);
            }


            if (detectGround)
            {
                lengths[i] = (hit.point - worldPosition).magnitude;
            } else
            {
                lengths[i] = length;
            }


            //wheels[i].transform.localPosition = new Vector3(wheelPositions[i].x, wheels[i].transform.localPosition.y, wheelPositions[i].z);
        }
        
        float frictionForce = mu * normalForce;
        Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.linearVelocity);
        localVelocity.z = 0;
        localVelocity.y = 0;
        Vector3 lateralVelocity = transform.TransformDirection(localVelocity);
        rigidBody.AddForce(-lateralVelocity * frictionForce * Time.deltaTime);
    }

}
