using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using Unity.Mathematics;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using System.Threading;

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
    public GameObject shootPosition;

    private InputAction moveAction;
    private InputAction attackAction;
    private float attackCooldown = 0.0f;

    public float speed = 100;
    public GameObject projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    // Update is called once per frame
    void Update()
    {
        attackCooldown -= Time.deltaTime;


        Vector3 cameraForward = Vector3.Scale(currentCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(currentCamera.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector2 moveDirection = moveAction.ReadValue<Vector2>();

        if (moveDirection != Vector2.zero)
        {
            bool isGrounded = GetComponent<Suspension>().isGrounded;
            
            if (isGrounded)
            {
                rigidBody.linearVelocity += transform.forward * Time.deltaTime * speed;
            }

            Vector3 baseDirection = cameraForward * moveDirection.y + cameraRight * moveDirection.x;
            Quaternion baseRotationTarget = Quaternion.LookRotation(baseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);
        }

        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            bool isAttacking = attackAction.ReadValue<float>() == 1.0f;
            if (isAttacking && attackCooldown <= 0.0f)
            {
                attackCooldown = 1.0f;

                GameObject bullet = Instantiate(projectile);
                bullet.transform.position = shootPosition.transform.position;
                Projectile projectileScript = bullet.GetComponent<Projectile>();
                Vector3 bulletDirection = (hit.point - shootPosition.transform.position).normalized;

                projectileScript.Shoot(bulletDirection * 1000.0f, 3.0f);
                projectileScript.onHit += (RaycastHit? hit) =>
                {
                    Destroy(bullet);
                };

            }


            Vector3 turretDirection = hit.point - turretAxis.transform.position;
            turretDirection.y = 0;
            turretDirection.Normalize();

            Quaternion rotationTarget = Quaternion.LookRotation(turretDirection);
            turretAxis.transform.rotation = Quaternion.Slerp(turretAxis.transform.rotation, rotationTarget, Time.deltaTime * turretRotationSpeed);
            turretAxis.transform.localEulerAngles = new Vector3(0, turretAxis.transform.localEulerAngles.y, 0);
        }
    }
}
