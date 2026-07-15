using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NavMeshAgent agent;
    public GameObject follow;

    public float baseRotationSpeed = 1.0f;
    private Rigidbody rigidBody;
    public float speed = 20.0f;

    private float updateInterval = 1.0f;
    private float timer = 0.0f;

    private float attackTimer = 0.0f;
    private float maxAttackTimer = 5.0f;

    private GameObject projectile;
    private HealthComponent healthComponent;

    public GameObject shootPosition;

    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();

        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.updateRotation = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        healthComponent = GetComponent<HealthComponent>();
        projectile = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Projectile.prefab");



        
    }

    // Update is called once per frame
    void Update()
    {
        if (healthComponent.isDead) { return; }

        timer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0.0f;
            agent.SetDestination(follow.transform.position);
        }



        agent.nextPosition = transform.position;
        Suspension suspension = GetComponent<Suspension>();

        bool isGrounded = suspension.isGrounded;

        if (isGrounded && agent.desiredVelocity != Vector3.zero)
        {
            suspension.rollResistance = 1.0f;
            rigidBody.linearVelocity += agent.desiredVelocity.normalized * Time.deltaTime * speed;

            Vector3 baseDirection = agent.desiredVelocity;
            baseDirection.y = 0;
            baseDirection.Normalize();

            Debug.DrawRay(transform.position, agent.desiredVelocity, Color.blue);

            Quaternion baseRotationTarget = Quaternion.LookRotation(baseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);
        }
        else
        {
            suspension.rollResistance = 10.0f;
        }

        Vector3 directionToPlayer = (follow.transform.position - transform.position).normalized;

        if (attackTimer >= maxAttackTimer)
        {
            attackTimer = 0.0f;

            GameObject bullet = Instantiate(projectile);
            bullet.transform.position = shootPosition.transform.position;
            Projectile projectileScript = bullet.GetComponent<Projectile>();
            Vector3 bulletDirection = directionToPlayer;


            projectileScript.ShootWithSpread(bulletDirection * 300.0f, 3.0f, 0.05f);
            projectileScript.onHit += (RaycastHit hit) =>
            {
                if (hit.transform.gameObject != null)
                {
                    HealthComponent healthComponent = hit.transform.gameObject.GetComponent<HealthComponent>();
                    if (healthComponent != null)
                    {
                        healthComponent?.TakeDamage(10.0f);
                    }
                }

                Destroy(bullet);
            };
        }
    }
}