
using System.Collections;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    [SerializeField]
    private float scopeFOV;
    [SerializeField]
    private float baseFOV;
    public UnityEngine.UI.Image crosshair;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        turnCameraAction = InputSystem.actions.FindAction("TurnCamera");
        turnCameraControllerXAction = InputSystem.actions.FindAction("TurnCameraControllerX");
        turnCameraControllerYAction = InputSystem.actions.FindAction("TurnCameraControllerY");
        zoomCameraAction = InputSystem.actions.FindAction("ZoomCamera");
        Camera camera = GetComponent<Camera>();

        camera.fieldOfView = baseFOV;

        turnCameraAction.started += (InputAction.CallbackContext callbackContext) =>
        {
            if (camera.fieldOfView == baseFOV)
            {
                LeanTween.value(gameObject, camera.fieldOfView, scopeFOV, 0.25f)
                .setOnUpdate((float value) =>
                {
                    camera.fieldOfView = value;
                });
            }
            else if (camera.fieldOfView == scopeFOV)
            {
                LeanTween.value(gameObject, camera.fieldOfView, baseFOV, 0.25f)
                .setOnUpdate((float value) =>
                {
                    camera.fieldOfView = value;
                });
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) { return; }

        Camera camera = GetComponent<Camera>();

        
        if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
        {
            float scopeSensitivity = camera.fieldOfView / baseFOV;

            cameraRotationY += Input.mousePositionDelta.x * Time.deltaTime * sensitivity * scopeSensitivity;
            cameraRotationX += -Input.mousePositionDelta.y * Time.deltaTime * sensitivity * scopeSensitivity;


            float moveDirectionX = turnCameraControllerXAction.ReadValue<float>();
            float moveDirectionY = turnCameraControllerYAction.ReadValue<float>();

            cameraRotationY += moveDirectionX * Time.deltaTime * sensitivity * scopeSensitivity;
            cameraRotationX -= moveDirectionY * Time.deltaTime * sensitivity * scopeSensitivity;
        }


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
