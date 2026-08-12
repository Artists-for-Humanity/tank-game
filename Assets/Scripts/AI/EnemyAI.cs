using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : Tank
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NavMeshAgent agent;
    public GameObject follow;
    
    public float experience = 1f;

    private float updateInterval = 1.0f;
    private float timer = 0.0f;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();

        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.updateRotation = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        healthComponent = GetComponent<HealthComponent>();
        LoadVehicle(currentVehicle);
        foreach (WeaponSlot weaponSlot in weaponSlots)
        {
            LoadWeapon(weaponSlot, null);
        }

        RefreshStats();
        UpdateFollow();
        healthComponent.onDied += () =>
        {
            Destroy(gameObject);

            LevelManager.Instance.AddExperience(experience);
        };

    }
    protected virtual void UpdateFollow()
    {
        follow = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (healthComponent.isDead) { return; }
        agent.nextPosition = transform.position;
       
        timer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0.0f;
            agent.SetDestination(follow.transform.position);
        }
        
        Suspension suspension = GetComponent<Suspension>();

        bool isGrounded = suspension.isGrounded;
        
        if (isGrounded && agent.desiredVelocity != Vector3.zero)
        {
            suspension.rollResistance = 1.0f;


            Debug.DrawRay(transform.position, agent.desiredVelocity, Color.blue);

            Move(agent.desiredVelocity.normalized);
            RotateBase(agent.desiredVelocity.normalized);
        }
        else
        {
            suspension.rollResistance = 10.0f;
        }

        Vector3 directionToPlayer = (follow.transform.position - transform.position).normalized;

        if (attackTimer >= weaponSlots[0].weaponUpgrade.firerate * weaponStatMultipiers.firerate)
        {
            attackTimer = 0.0f;

            ShootGun(follow.transform.position, 1 << gameObject.layer);
        }

        PointGun((follow.transform.position - transform.position).normalized);
    }
}