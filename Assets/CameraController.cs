using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    public GameObject target;

    private InputAction turnCameraAction;
    private float cameraRotationY;
    private float cameraRotationX;
    
    public float sensitivity = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnCameraAction = InputSystem.actions.FindAction("TurnCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {return;}

        bool turningCamera = turnCameraAction.ReadValue<float>() == 1.0f;
        
        if (turningCamera)
        {
            cameraRotationY += Input.mousePositionDelta.x * Time.deltaTime * sensitivity;
            cameraRotationX += -Input.mousePositionDelta.y * Time.deltaTime * sensitivity;
        }

        transform.position = target.transform.position;
        transform.rotation = quaternion.Euler(cameraRotationX, cameraRotationY, 0);
        transform.position -= transform.forward * 10;
    }
}
