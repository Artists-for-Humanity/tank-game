

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
    private GameObject weaponAxis;
    //public GameObject barrelAxis;
    private GameObject projectile;
    public float baseRotationSpeed = 5;
    public float turretRotationSpeed = 5;

    public Camera currentCamera;
    public GameObject shootPosition;

    private InputAction moveAction;
    private InputAction attackAction;
    private float attackTimer = 0.0f;

    public float bulletDamage = 50.0f;
    public float firerate = 1.0f;
    public float vehicleSpeed = 100f;
    public float bulletSpeed = 1000f;
    public float bulletLifetime = 3f;
    public float bulletSpread = 0f;


    private HealthComponent healthComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");

        weaponAxis = transform.Find("WeaponAxis").gameObject;
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
                rigidBody.linearVelocity += transform.forward * Time.deltaTime * vehicleSpeed;
            }

            Vector3 baseDirection = cameraForward * moveDirection.y + cameraRight * moveDirection.x;
            Quaternion baseRotationTarget = Quaternion.LookRotation(baseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);
        }


        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        bool isAttacking = attackAction.ReadValue<float>() == 1.0f;
        if (isAttacking && attackTimer <= 0.0f)
        {
            attackTimer = firerate;

            ShootGun();
        }


        Vector3 turretDirection = ray.direction;
        turretDirection.y = 0;
        turretDirection.Normalize();

        Quaternion rotationTarget = Quaternion.LookRotation(turretDirection);
        weaponAxis.transform.rotation = Quaternion.Slerp(weaponAxis.transform.rotation, rotationTarget, Time.deltaTime * turretRotationSpeed);
        weaponAxis.transform.localEulerAngles = new Vector3(0, weaponAxis.transform.localEulerAngles.y, 0);

    }

    void ShootGun()
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        GameObject bullet = Instantiate(projectile);
        bullet.transform.position = shootPosition.transform.position;
        Projectile projectileScript = bullet.GetComponent<Projectile>();
        Vector3 bulletDirection = ray.direction;


        projectileScript.ShootWithSpread(bulletDirection * bulletSpeed, bulletLifetime, bulletSpread, 1 << gameObject.layer);
        projectileScript.onHit += (RaycastHit hit) =>
        {
            if (hit.transform.gameObject != null)
            {
                HealthComponent enemyHealthComponent = hit.transform.gameObject.GetComponent<HealthComponent>();
                if (enemyHealthComponent != null)
                {
                    enemyHealthComponent?.TakeDamage(bulletDamage);
                }
            }
        };
    }
}
