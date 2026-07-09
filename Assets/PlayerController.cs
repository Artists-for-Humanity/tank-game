using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject turretAxis;
    //public GameObject barrelAxis;

    public Vector2 turretRotationLimit = new Vector2(-135, 135);
    //public Vector2 barrelRotationLimit = new Vector2(-45, 45);

    public float baseRotationSpeed = 5;
    public float turretRotationSpeed = 5;

    public Camera currentCamera;

    private InputAction moveAction;

    public float speed = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {


        Vector3 cameraForward = Vector3.Scale(currentCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(currentCamera.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector2 moveDirection = moveAction.ReadValue<Vector2>();

        if (moveDirection != Vector2.zero)
        {
            bool isGrounded = Physics.Raycast(transform.position, -transform.up, 1);
            if (isGrounded)
            {
                rigidBody.linearVelocity += transform.forward * Time.deltaTime * speed;
            }

            //(cameraForward * moveDirection.y + cameraRight * moveDirection.x) * Time.deltaTime * 10;

            Vector3 baseDirection = cameraForward * moveDirection.y + cameraRight * moveDirection.x;
            Quaternion baseRotationTarget = Quaternion.LookRotation(baseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);


        }

        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 turretDirection = hit.point - turretAxis.transform.position;
            turretDirection.y = 0;
            turretDirection.Normalize();

            Quaternion rotationTarget = Quaternion.LookRotation(turretDirection);
            turretAxis.transform.rotation = Quaternion.Slerp(turretAxis.transform.rotation, rotationTarget, Time.deltaTime * turretRotationSpeed);
            turretAxis.transform.localEulerAngles = new Vector3(0, turretAxis.transform.localEulerAngles.y, 0);
        }
    }
}
