using UnityEngine;

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

    public float bulletDamage = 50.0f;
    public float firerate = 1.0f;
    public float vehicleSpeed = 100f;
    public float bulletSpeed = 1000f;
    public float bulletLifetime = 3f;
    public float bulletSpread = 0f;
    public int bulletsPerShot = 1;

    
    void LoadVehicle(VehicleUpgrade vehicle)
    {
        
    }

    void LoadWeapon(WeaponUpgrade weapon)
    {
        
    }


    void PointBase(Vector3 direction)
    {
        rigidBody.linearVelocity += transform.forward * Time.deltaTime * vehicleSpeed;

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
        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject bullet = Instantiate(projectile);
            bullet.transform.position = shootPosition.transform.position;
            Projectile projectileScript = bullet.GetComponent<Projectile>();

            projectileScript.ShootWithSpread(direction * bulletSpeed, bulletLifetime, bulletSpread, 1 << gameObject.layer, 10);
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
}
