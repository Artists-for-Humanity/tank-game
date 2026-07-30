using UnityEngine;

public struct TankStats
{
    public float health;
    public float bulletDamage;
    public float bulletSpeed;
    public float firerate;
    public float vehicleSpeed;

    public float bulletLifetime;
    public float bulletSpread;
    public int bulletsPerShot;
}

public class Tank : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Rigidbody rigidBody;
    private GameObject weaponAxis;
    private GameObject projectile;
    public GameObject shootPosition;
    private HealthComponent healthComponent;

    public float baseRotationSpeed = 5;
    public float turretRotationSpeed = 5;

    private float attackTimer = 0.0f;

    public TankStats tankStats = new TankStats()
    {
        health = 100,
        bulletDamage = 50.0f,
        firerate = 1.0f,
        vehicleSpeed = 100f,
        bulletSpeed = 1000f,
        bulletLifetime = 3f,
        bulletSpread = 0f,
        bulletsPerShot = 1,
    };

    

    void Move(Vector3 direction)
    {
        rigidBody.linearVelocity += transform.forward * Time.deltaTime * tankStats.vehicleSpeed;

        Quaternion baseRotationTarget = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, baseRotationTarget, Time.deltaTime * baseRotationSpeed);
    }
    void PointGun(Vector3 direction)
    {

        direction.y = 0;
        direction.Normalize();

        Quaternion rotationTarget = Quaternion.LookRotation(direction);
        weaponAxis.transform.rotation = Quaternion.Slerp(weaponAxis.transform.rotation, rotationTarget, Time.deltaTime * turretRotationSpeed);
        weaponAxis.transform.localEulerAngles = new Vector3(0, weaponAxis.transform.localEulerAngles.y, 0);
    }
    void ShootGun(Vector3 direction)
    {
        for (int i = 0; i < tankStats.bulletsPerShot; i++)
        {
            GameObject bullet = Instantiate(projectile);
            bullet.transform.position = shootPosition.transform.position;
            Projectile projectileScript = bullet.GetComponent<Projectile>();

            projectileScript.ShootWithSpread(direction * tankStats.bulletSpeed, tankStats.bulletLifetime, tankStats.bulletSpread, 1 << gameObject.layer, 10);
            projectileScript.onHit += (RaycastHit hit) =>
            {
                if (hit.transform.gameObject != null)
                {
                    HealthComponent enemyHealthComponent = hit.transform.gameObject.GetComponent<HealthComponent>();
                    if (enemyHealthComponent != null)
                    {
                        enemyHealthComponent?.TakeDamage(tankStats.bulletDamage);
                    }
                }
            };
        }

    }
}
