using UnityEngine;

public class LocalTransformConstraint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool positionX = false;
    public bool positionY = false;
    public bool positionZ = false;

    public bool rotationX = false;
    public bool rotationY = false;
    public bool rotationZ = false;

    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        float newPositionX, newPositionY, newPositionZ;
        float newRotationX, newRotationY, newRotationZ;

        if (positionX)
        {
            newPositionX = 0;
        } else
        {
            newPositionX = transform.localPosition.x;
        }

        if (positionY)
        {
            newPositionY = 0;
        } else
        {
            newPositionY = transform.localPosition.y;
        }

        if (positionZ)
        {
            newPositionZ = 0;
        } else
        {
            newPositionZ = transform.localPosition.z;
        }


        if (rotationX)
        {
            newRotationX = 0;
        } else
        {
            newRotationX = transform.localRotation.x;
        }

        if (rotationY)
        {
            newRotationY = 0;
        } else
        {
            newRotationY = transform.localRotation.y;
        }

        if (rotationZ)
        {
            newRotationZ = 0;
        } else
        {
            newRotationZ = transform.localRotation.z;
        }

        transform.localPosition = new Vector3(newPositionX, newPositionY, newPositionZ);
        transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, newRotationZ);
    }
}
