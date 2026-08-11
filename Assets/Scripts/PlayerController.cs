

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
using Unity.XR.OpenVR;

public class PlayerController : Tank
{
    public Camera currentCamera;

    private InputAction moveAction;
    private InputAction attackAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        moveAction = InputSystem.actions.FindAction("Move");
        attackAction = InputSystem.actions.FindAction("Attack");

        rigidBody = GetComponent<Rigidbody>();

        healthComponent = GetComponent<HealthComponent>();

        healthComponent.healthChanged += (float oldHealth, float newHealth) =>
        {
            UIManager.UpdateHealthBar(healthComponent.HealthAsPercentage());
        };

        LoadVehicle(currentVehicle);
        LoadWeapon(weaponSlots[0], null);
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer -= Time.deltaTime;

        UIManager.UpdateReloadBar(attackTimer / (weaponSlots[0].weaponUpgrade.firerate * weaponStatMultipiers.firerate));

        Vector3 cameraForward = Vector3.Scale(currentCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(currentCamera.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        bool isGrounded = GetComponent<Suspension>().isGrounded;

        if (moveDirection != Vector2.zero)
        {
            Vector3 baseDirection = cameraForward * moveDirection.y + cameraRight * moveDirection.x;
            RotateBase(baseDirection);

            if (isGrounded)
            {
                Move(transform.forward * Vector3.Dot(transform.forward, baseDirection));
            }
        }

        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        Vector3 bulletDirection = ray.direction;
        Vector3 to = transform.position + bulletDirection;

        if (Physics.Raycast(ray, out mouseHit))
        {
            to = mouseHit.point;
        }

        bool isAttacking = attackAction.ReadValue<float>() == 1.0f;
        if (isAttacking && attackTimer <= 0.0f)
        {
            attackTimer = weaponSlots[0].weaponUpgrade.firerate * weaponStatMultipiers.firerate;

            ShootGun(to, 1 << gameObject.layer);
        }

        Vector3 turretDirection = ray.direction;
        turretDirection.y = 0;
        turretDirection.Normalize();

        PointGun(turretDirection);
    }
}
