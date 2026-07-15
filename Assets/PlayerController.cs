

using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
using Unity.Mathematics;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using System.Threading;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidBody;
    private GameObject turretAxis;
    //public GameObject barrelAxis;
    private GameObject projectile;
    public float baseRotationSpeed = 5;
    public float turretRotationSpeed = 5;

    public Camera currentCamera;
    public GameObject shootPosition;

    private InputAction moveAction;
    private InputAction attackAction;
    private float attackTimer = 0.0f;
    public float attackCooldown = 0.1f;

    public float speed = 100;


    private HealthComponent healthComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");

        turretAxis = transform.Find("TurretAxis").gameObject;
        projectile = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Projectile.prefab");

        rigidBody = GetComponent<Rigidbody>();

        healthComponent = GetComponent<HealthComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer -= Time.deltaTime;


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


        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        bool isAttacking = attackAction.ReadValue<float>() == 1.0f;
        if (isAttacking && attackTimer <= 0.0f)
        {
            attackTimer = attackCooldown;

            ShootGun();
        }


        Vector3 turretDirection = ray.direction;
        turretDirection.y = 0;
        turretDirection.Normalize();

        Quaternion rotationTarget = Quaternion.LookRotation(turretDirection);
        turretAxis.transform.rotation = Quaternion.Slerp(turretAxis.transform.rotation, rotationTarget, Time.deltaTime * turretRotationSpeed);
        turretAxis.transform.localEulerAngles = new Vector3(0, turretAxis.transform.localEulerAngles.y, 0);

    }

    void ShootGun()
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        GameObject bullet = Instantiate(projectile);
        bullet.transform.position = shootPosition.transform.position;
        Projectile projectileScript = bullet.GetComponent<Projectile>();
        Vector3 bulletDirection = ray.direction;


        projectileScript.ShootWithSpread(bulletDirection * 1000.0f, 3.0f, 0.01f);
        projectileScript.onHit += (RaycastHit hit) =>
        {
            if (hit.transform.gameObject != null)
            {
                HealthComponent enemyHealthComponent = hit.transform.gameObject.GetComponent<HealthComponent>();
                if (enemyHealthComponent != null)
                {
                    enemyHealthComponent?.TakeDamage(50.0f);
                }
            }
        };
    }
}
