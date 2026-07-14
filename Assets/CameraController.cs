
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class CameraController : MonoBehaviour
{
    public GameObject target;

    private InputAction turnCameraAction;
    private InputAction turnCameraControllerXAction;
    private InputAction turnCameraControllerYAction;
    private InputAction zoomCameraAction;
    private float cameraRotationY;
    private float cameraRotationX;
    public float cameraYOffset = 0f;
    public float sensitivity = 1.0f;
    public float zoomSensitivity = 1.0f;
    private float zoom = 10.0f;


    public UnityEngine.UI.Image crosshair;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnCameraAction = InputSystem.actions.FindAction("TurnCamera");
        turnCameraControllerXAction = InputSystem.actions.FindAction("TurnCameraControllerX");
        turnCameraControllerYAction = InputSystem.actions.FindAction("TurnCameraControllerY");
        zoomCameraAction = InputSystem.actions.FindAction("ZoomCamera");
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

        float moveDirectionX = turnCameraControllerXAction.ReadValue<float>();
        float moveDirectionY = turnCameraControllerYAction.ReadValue<float>();
        
        cameraRotationY += moveDirectionX * Time.deltaTime * sensitivity;
        cameraRotationX -= moveDirectionY * Time.deltaTime * sensitivity;

        transform.position = target.transform.position + Vector3.up * cameraYOffset;
        transform.rotation = quaternion.Euler(cameraRotationX, cameraRotationY, 0);
        transform.position -= transform.forward * zoom;

        if (Mouse.current.delta.magnitude > 0)
        {
            crosshair.transform.position = new Vector3(Mouse.current.position.x.value, Mouse.current.position.y.value, 0);
        }

        float zoomDirectionY = zoomCameraAction.ReadValue<float>();
        zoom -= zoomDirectionY * Time.deltaTime * zoomSensitivity;

    }
    
    
}
