
using UnityEngine;
using UnityEngine.AI;

public class HelicopterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject follow;
    private Rigidbody rigidBody;
    public HealthComponent healthComponent;

    private float rotatePercent = 0f;

    private GameObject mainRotorAxis;
    private GameObject extraRotorAxis;

    [SerializeField]
    private float hoverHeight = 50f;
    [SerializeField]
    private float hoverStrength = 5f;
    [SerializeField]
    private float damping = 5f;
    [SerializeField]
    private float moveStrength = 5f;

    [SerializeField]
    private GameObject[] shootPositions;
    public GameObject projectile;
    [SerializeField]
    WeaponStats helicopterWeaponStats;


    public AudioClip fireSound;
    public AudioClip impactSound;

    public float experience;
    float attackTimer = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainRotorAxis = transform.Find("MainRotorAxis").gameObject;
        extraRotorAxis = transform.Find("ExtraRotorAxis").gameObject;

        agent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();

        agent.updateUpAxis = false;
        agent.updatePosition = false;
        agent.updateRotation = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        
        follow = GameObject.FindGameObjectWithTag("Player");
        healthComponent.onDied += () =>
        {
            Destroy(gameObject);

            LevelManager.Instance.AddExperience(experience);
        };

        healthComponent.maxHealth *= 1f + (float)(Spawner.Instance.wave / 10) * 0.2f;
        healthComponent.health = healthComponent.maxHealth;
    }

    void Update()
    {
        rotatePercent += Time.deltaTime * 4f;
        rotatePercent %= 1f;

        attackTimer += Time.deltaTime;
        
        mainRotorAxis.transform.localEulerAngles = new Vector3(0, rotatePercent * 360f, 0);
        extraRotorAxis.transform.localEulerAngles = new Vector3(rotatePercent * 360f, 0, 0);

        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        agent.nextPosition = transform.position;
        agent.SetDestination(follow.transform.position);

        Vector3 vertical = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f * hoverHeight, ~(1 << gameObject.layer)))
        {
            Vector3 goal = hit.point + Vector3.up * hoverHeight;

            Vector3 diff = (goal - transform.position);
            
            Vector3 dampingVec = rigidBody.linearVelocity * damping;

            vertical = diff * hoverStrength - dampingVec;
        }

        rigidBody.AddForce((agent.desiredVelocity * moveStrength + vertical) * rigidBody.mass * Time.fixedDeltaTime);

        RotateBase((follow.transform.position - transform.position).normalized);

        if (attackTimer > helicopterWeaponStats.firerate)
        {
            attackTimer = 0f;

            ShootGun(follow.transform.position, 1 << gameObject.layer);
        }
    }

    void RotateBase(Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();
        
        Quaternion diff = Quaternion.LookRotation(direction) * Quaternion.Inverse(transform.rotation);

        diff.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
        {
            angle -= 360f;
        }

        float angleRad = angle * Mathf.Deg2Rad;

        Vector3 goalAngularVelocity = axis.normalized * angleRad * 50;
        rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, goalAngularVelocity, Time.fixedDeltaTime * 50);
    }

    void ShootGun(Vector3 to, LayerMask layerMask)
    {
        foreach (GameObject shootPosition in shootPositions)
        {

            AudioSource audioSource = AudioUtils.PlayClipAt(fireSound, transform.position);
            audioSource.volume = 0.5f;
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.minDistance = 10f;
            audioSource.maxDistance = 100f;

            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.spatialBlend = 1;


            for (int i = 0; i < helicopterWeaponStats.bulletsPerShot; i++)
            {
                GameObject bullet = Instantiate(projectile);

                Projectile projectileScript = bullet.GetComponent<Projectile>();
                

                projectileScript.Shoot(
                    shootPosition.transform.position, 
                    to, 
                    helicopterWeaponStats.bulletSpeed, 
                    helicopterWeaponStats.bulletLifetime, 
                    helicopterWeaponStats.bulletSpread,  
                    layerMask, 
                    1
                    );


                projectileScript.onHit += (RaycastHit hit) =>
                {
                    if (hit.transform.gameObject != null)
                    {
                        HealthComponent enemyHealthComponent = hit.transform.gameObject.GetComponent<HealthComponent>();
                        if (enemyHealthComponent != null)
                        {
                            enemyHealthComponent?.TakeDamage(helicopterWeaponStats.bulletDamage);
                        }

                        AudioSource impactAudioSource = AudioUtils.PlayClipAt(impactSound, hit.point);
                            impactAudioSource.volume = 0.2f;
                            impactAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                            impactAudioSource.minDistance = 0f;
                            impactAudioSource.maxDistance = 75f;

                            impactAudioSource.rolloffMode = AudioRolloffMode.Linear;
                            impactAudioSource.spatialBlend = 1;
                    }
                };
            }
        }
    }
}
